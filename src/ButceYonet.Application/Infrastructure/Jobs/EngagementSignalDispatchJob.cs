using System.Globalization;
using System.Text.Json;
using ButceYonet.Application.Domain.Entities;
using ButceYonet.Application.Domain.Enums;
using ButceYonet.Application.Infrastructure.Data;
using ButceYonet.Application.Infrastructure.Notifications;
using DotBoil.EFCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace ButceYonet.Application.Infrastructure.Jobs;

// Faz II: EngagementSignalJob ve TransactionCreatedDomainEventConsumer'ın ürettiği sinyalleri
// (IsSent = false) kullanıcının dilinde push notification olarak gönderir.
//
// Bir sinyal "işlendi" sayılıp IsSent = true / SentAt = now olarak işaretlenir. Bu şu durumları kapsar:
//   - push gönderildi,
//   - kullanıcının MobileNotification kanalı kapalı ya da kayıtlı cihazı yok,
//   - sinyal MaxSignalAge'den eski (örn. ilk deploy'da biriken eski sinyaller "dün 7 günlük seri" gibi
//     bayat bildirimler olarak gitmesin).
// FCM çağrısı exception fırlatırsa sinyal işaretlenmez, bir sonraki turda tekrar denenir.
public class EngagementSignalDispatchJob : BackgroundService
{
    private const int BatchSize = 200;
    private static readonly TimeSpan Interval = TimeSpan.FromMinutes(5);
    private static readonly TimeSpan MaxSignalAge = TimeSpan.FromHours(48);

    private readonly ILogger<EngagementSignalDispatchJob> _logger;
    private readonly IServiceProvider _serviceProvider;

    public EngagementSignalDispatchJob(
        ILogger<EngagementSignalDispatchJob> logger,
        IServiceProvider serviceProvider)
    {
        _logger = logger;
        _serviceProvider = serviceProvider;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                await RunAsync(stoppingToken);
            }
            catch (OperationCanceledException)
            {
                _logger.LogInformation("Engagement signal dispatch job timed out.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Engagement signal dispatch job failed.");
            }

            await Task.Delay(Interval, stoppingToken);
        }
    }

    private async Task RunAsync(CancellationToken cancellationToken)
    {
        using var scope = _serviceProvider.CreateAsyncScope();
        var signalRepository = scope.ServiceProvider.GetRequiredService<IRepository<EngagementSignal, ButceYonetDbContext>>();
        var currencyRepository = scope.ServiceProvider.GetRequiredService<IRepository<Currency, ButceYonetDbContext>>();
        var recipientProvider = scope.ServiceProvider.GetRequiredService<INotificationRecipientProvider>();
        var pushSender = scope.ServiceProvider.GetRequiredService<IPushNotificationSender>();

        var signals = await signalRepository
            .GetAll()
            .Where(s => !s.IsSent)
            .OrderBy(s => s.OccurredAt)
            .Take(BatchSize)
            .ToListAsync(cancellationToken);

        if (signals.Count == 0)
            return;

        var currencyMap = await currencyRepository.GetAll().ToDictionaryAsync(c => c.Id, cancellationToken);
        var recipients = await recipientProvider.GetManyAsync(signals.Select(s => s.UserId), cancellationToken);
        var staleThreshold = DateTime.UtcNow - MaxSignalAge;
        var processedIds = new List<int>();

        foreach (var signal in signals)
        {
            if (signal.OccurredAt < staleThreshold || !recipients.TryGetValue(signal.UserId, out var recipient))
            {
                processedIds.Add(signal.Id);
                continue;
            }

            try
            {
                var args = BuildArgs(signal, recipient.Culture, currencyMap);
                var key = $"EngagementSignal.{signal.Type}";

               await pushSender.SendAsync(
                    recipient,
                    $"{key}.Title",
                    $"{key}.Body",
                    args,
                    BuildData(signal),
                    cancellationToken);

                processedIds.Add(signal.Id);
            }
            catch (JsonException ex)
            {
                // Bozuk payload tekrar denenince düzelmez; işaretleyip geç.
                _logger.LogWarning(ex, "Engagement signal {SignalId} has an invalid payload, skipping.", signal.Id);
                processedIds.Add(signal.Id);
            }
            catch (Exception ex) when (ex is not OperationCanceledException)
            {
                _logger.LogError(ex, "Engagement signal {SignalId} could not be sent, will retry.", signal.Id);
            }
        }

        if (processedIds.Count == 0)
            return;

        var now = DateTime.UtcNow;
        await signalRepository
            .GetAll()
            .Where(s => processedIds.Contains(s.Id))
            .ExecuteUpdateAsync(setters => setters
                .SetProperty(s => s.IsSent, true)
                .SetProperty(s => s.SentAt, now), cancellationToken);
    }

    // Localizations'taki "EngagementSignal.{Type}.Title/Body" metinleri bu sırayla {0}, {1}, {2} alır.
    private static object[] BuildArgs(EngagementSignal signal, CultureInfo culture, Dictionary<int, Currency> currencyMap)
    {
        switch (signal.Type)
        {
            case EngagementSignalType.GoalProgressMilestone:
            case EngagementSignalType.GoalCompleted:
            {
                var payload = Deserialize<GoalProgressSignalPayload>(signal);
                return new object[] { payload.GoalName, payload.Percentage };
            }
            case EngagementSignalType.GoalDeadlineApproaching:
            {
                var payload = Deserialize<GoalDeadlineApproachingSignalPayload>(signal);
                return new object[]
                {
                    payload.GoalName,
                    payload.DaysRemaining,
                    FormatAmount(payload.RemainingAmount, payload.CurrencyId, culture, currencyMap)
                };
            }
            case EngagementSignalType.StreakMilestone:
            {
                var payload = Deserialize<StreakMilestoneSignalPayload>(signal);
                return new object[] { payload.StreakDays };
            }
            case EngagementSignalType.InactivityWarning:
            {
                var payload = Deserialize<InactivityWarningSignalPayload>(signal);
                return new object[] { payload.DaysSinceLastTransaction };
            }
            case EngagementSignalType.AnomalousSpending:
            {
                var payload = Deserialize<AnomalousSpendingSignalPayload>(signal);
                return new object[]
                {
                    payload.TransactionName,
                    FormatAmount(payload.Amount, payload.CurrencyId, culture, currencyMap),
                    payload.Ratio.ToString("0.#", culture)
                };
            }
            default:
                return Array.Empty<object>();
        }
    }

    // Mobil uygulamanın bildirime tıklanınca ilgili ekrana gidebilmesi için.
    private static Dictionary<string, string> BuildData(EngagementSignal signal)
    {
        var data = new Dictionary<string, string>
        {
            ["type"] = signal.Type.ToString(),
            ["signalId"] = signal.Id.ToString()
        };

        if (signal.GoalId.HasValue)
            data["goalId"] = signal.GoalId.Value.ToString();

        if (signal.TransactionId.HasValue)
            data["transactionId"] = signal.TransactionId.Value.ToString();

        return data;
    }

    private static T Deserialize<T>(EngagementSignal signal)
    {
        return JsonSerializer.Deserialize<T>(signal.PayloadJson)
               ?? throw new JsonException($"Payload of signal {signal.Id} is empty.");
    }

    private static string FormatAmount(decimal amount, int currencyId, CultureInfo culture, Dictionary<int, Currency> currencyMap)
    {
        var formatted = amount.ToString("N2", culture);
        var currency = currencyMap.GetValueOrDefault(currencyId);

        if (currency is null)
            return formatted;

        return currency.IsSymbolRight ? $"{formatted} {currency.Symbol}" : $"{currency.Symbol} {formatted}";
    }
}
