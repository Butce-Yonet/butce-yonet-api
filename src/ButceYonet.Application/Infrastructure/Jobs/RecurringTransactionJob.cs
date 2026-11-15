using System.Text.Json;
using ButceYonet.Application.Application.Interfaces;
using ButceYonet.Application.Domain.Entities;
using ButceYonet.Application.Domain.Enums;
using ButceYonet.Application.Domain.Events;
using ButceYonet.Application.Infrastructure.Data;
using ButceYonet.Application.Infrastructure.MailTemplates;
using ButceYonet.Application.Infrastructure.Notifications;
using ButceYonet.Application.Infrastructure.Services;
using DotBoil.EFCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using User = ButceYonet.Application.Domain.Entities.User;

namespace ButceYonet.Application.Infrastructure.Jobs;

public class RecurringTransactionJob : BackgroundService
{
    // Tek bir kayit icin bir kosuda islenecek azami occurrence sayisi; cok eski tarihli
    // kayitlarin tek seferde devasa islem uretmesini engeller.
    private const int MaxOccurrencesPerRun = 500;

    private readonly ILogger<RecurringTransactionJob> _logger;
    private readonly IServiceProvider _serviceProvider;

    public RecurringTransactionJob(
        ILogger<RecurringTransactionJob> logger,
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
                await RunDailyAsync(stoppingToken);
            }
            catch (OperationCanceledException)
            {
                _logger.LogInformation("Recurring transaction job timed out.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Recurring transaction job timed out.");
            }

            await Task.Delay(TimeSpan.FromMinutes(5), stoppingToken);
        }
    }

    private async Task RunDailyAsync(CancellationToken cancellationToken)
    {
        using var scope = _serviceProvider.CreateAsyncScope();
        var recurringTransactionRepository = scope.ServiceProvider.GetService<IRepository<RecurringTransaction, ButceYonetDbContext>>();
        var transactionRepository = scope.ServiceProvider.GetService<IRepository<TransactionV2, ButceYonetDbContext>>();
        var recurringTransactionIntervalsService = scope.ServiceProvider.GetService<IRecurringTransactionIntervalsService>();
        var notebookPeriodResolver = scope.ServiceProvider.GetService<INotebookPeriodResolver>();

        var today = DateTime.UtcNow.Date;
        var endOfDay = today.AddDays(1).AddTicks(-1);

        // NextOccurrence'i bugun ve oncesine dusen tum kayitlar alinir. Boylece gecmis tarihli
        // baslangic girilen ya da job'un calismadigi gunlerde vadesi gecen kayitlar da yakalanir.
        // EndDate kontrolu bugune gore yapilir: EndDate'i bugunden onceki, yani zaten tamamen
        // sona ermis seriler sorguya hic girmez - aksi halde uzun sure once bitmis bir seri
        // aylar sonra "yeniden canlanip" gecmis tarihli islem ve seri-bitti maili uretebilir.
        var recurringTransactions = await recurringTransactionRepository
            .GetAll()
            .Where(rt =>
                rt.NextOccurrence.HasValue &&
                rt.NextOccurrence.Value <= endOfDay &&
                (!rt.EndDate.HasValue || rt.EndDate.Value.Date >= today))
            .ToListAsync();

        var processedItemsByUser = new Dictionary<int, List<(string Name, string Description, decimal Amount, int CurrencyId, TransactionTypes TransactionType)>>();
        var endedItemsByUser = new Dictionary<int, List<(string Name, string Description, decimal Amount, int CurrencyId, TransactionTypes TransactionType)>>();

        foreach (var recurringTransaction in recurringTransactions)
        {
            var transactions = JsonSerializer.Deserialize<List<TransactionV2>>(recurringTransaction.StateData);
            var firstTransaction = transactions?.FirstOrDefault();

            var processedOccurrenceCount = 0;

            // Vadesi gecmis occurrence'lar bugune kadar sirayla islenir; her biri kendi tarihine
            // ait deftere ve kendi tarihiyle yazilir.
            while (recurringTransaction.NextOccurrence.HasValue &&
                   recurringTransaction.NextOccurrence.Value <= endOfDay &&
                   !IsAfterEndDate(recurringTransaction, recurringTransaction.NextOccurrence.Value))
            {
                var occurrenceDate = recurringTransaction.NextOccurrence.Value;

                var nextOccurrence = recurringTransactionIntervalsService.CalculateInterval(
                    occurrenceDate,
                    recurringTransaction.Frequency,
                    recurringTransaction.Interval);

                // Hatali konfigurasyon (or. Interval = 0) tarihi ilerletmez; sonsuz donguyu ve
                // mukerrer islem uretimini onlemek icin kayit atlanir.
                if (nextOccurrence <= occurrenceDate)
                {
                    _logger.LogError(
                        "Recurring transaction {RecurringTransactionId} has a non-advancing interval (Frequency: {Frequency}, Interval: {Interval}). Skipping.",
                        recurringTransaction.Id,
                        recurringTransaction.Frequency,
                        recurringTransaction.Interval);
                    break;
                }

                if (processedOccurrenceCount >= MaxOccurrencesPerRun)
                {
                    _logger.LogWarning(
                        "Recurring transaction {RecurringTransactionId} reached the per-run occurrence limit ({Limit}). Remaining occurrences will be processed on the next run.",
                        recurringTransaction.Id,
                        MaxOccurrencesPerRun);
                    break;
                }

                if (firstTransaction is not null)
                {
                    var notebook = await notebookPeriodResolver.ResolveOrCreateAsync(recurringTransaction.UserId, occurrenceDate, cancellationToken);

                    var transaction = new TransactionV2()
                    {
                        NotebookV2Id = notebook.Id,
                        ExternalId = Guid.NewGuid().ToString(),
                        Name = firstTransaction.Name,
                        Description = firstTransaction.Description,
                        Amount = firstTransaction.Amount,
                        CurrencyId = firstTransaction.CurrencyId,
                        TransactionType = firstTransaction.TransactionType,
                        TransactionDate = occurrenceDate
                    };

                    transaction.TransactionLabelsV2 = (firstTransaction.TransactionLabelsV2 ?? new List<TransactionLabelV2>())
                        .Select(tl => new TransactionLabelV2
                        {
                            UserLabelId = tl.UserLabelId
                        }).ToList();

                    transaction.IsMatched = transaction.TransactionLabelsV2.Any();

                    var transactionCreatedDomainEvent = new TransactionCreatedDomainEvent(transaction);
                    transaction.AddEvent(transactionCreatedDomainEvent);
                    await transactionRepository.AddAsync(transaction);

                    if (!processedItemsByUser.TryGetValue(recurringTransaction.UserId, out var list))
                    {
                        list = new List<(string Name, string Description, decimal Amount, int CurrencyId, TransactionTypes TransactionType)>();
                        processedItemsByUser[recurringTransaction.UserId] = list;
                    }
                    list.Add((recurringTransaction.Name, firstTransaction.Description ?? "", firstTransaction.Amount, firstTransaction.CurrencyId, firstTransaction.TransactionType));
                }

                recurringTransaction.NextOccurrence = nextOccurrence;
                processedOccurrenceCount++;
            }

            if (processedOccurrenceCount == 0)
                continue;

            recurringTransactionRepository.Update(recurringTransaction);

            // Son hesaplanan NextOccurrence EndDate'i gectiyse bu kayit icin islenen son
            // occurrence'di - kullaniciya seri bitis bildirimi gonderilecek.
            if (firstTransaction is not null && HasSeriesEnded(recurringTransaction))
            {
                if (!endedItemsByUser.TryGetValue(recurringTransaction.UserId, out var endedList))
                {
                    endedList = new List<(string Name, string Description, decimal Amount, int CurrencyId, TransactionTypes TransactionType)>();
                    endedItemsByUser[recurringTransaction.UserId] = endedList;
                }
                endedList.Add((recurringTransaction.Name, firstTransaction.Description ?? "", firstTransaction.Amount, firstTransaction.CurrencyId, firstTransaction.TransactionType));
            }
        }

        await recurringTransactionRepository.SaveChangesAsync();

        await SendRecurringProcessedNotificationEmailsAsync(scope.ServiceProvider, processedItemsByUser, today, cancellationToken);
        await SendRecurringEndedNotificationEmailsAsync(scope.ServiceProvider, endedItemsByUser, today, cancellationToken);
    }

    private static bool HasSeriesEnded(RecurringTransaction recurringTransaction)
    {
        return recurringTransaction.NextOccurrence.HasValue &&
               IsAfterEndDate(recurringTransaction, recurringTransaction.NextOccurrence.Value);
    }

    private static bool IsAfterEndDate(RecurringTransaction recurringTransaction, DateTime occurrenceDate)
    {
        return recurringTransaction.EndDate.HasValue &&
               occurrenceDate.Date > recurringTransaction.EndDate.Value.Date;
    }

    private async Task SendRecurringProcessedNotificationEmailsAsync(
        IServiceProvider serviceProvider,
        Dictionary<int, List<(string Name, string Description, decimal Amount, int CurrencyId, TransactionTypes TransactionType)>> processedItemsByUser,
        DateTime processedDate,
        CancellationToken cancellationToken)
    {
        if (processedItemsByUser.Count == 0)
            return;

        var currencyRepository = serviceProvider.GetService<IRepository<Currency, ButceYonetDbContext>>();
        var recipientProvider = serviceProvider.GetRequiredService<INotificationRecipientProvider>();
        var mailSender = serviceProvider.GetRequiredService<IMailNotificationSender>();
        var pushSender = serviceProvider.GetRequiredService<IPushNotificationSender>();

        var currencyMap = await currencyRepository.GetAll()
            .ToDictionaryAsync(c => c.Id, cancellationToken);

        var recipients = await recipientProvider.GetManyAsync(processedItemsByUser.Keys, cancellationToken);

        foreach (var (userId, items) in processedItemsByUser)
        {
            if (items.Count == 0 || !recipients.TryGetValue(userId, out var recipient))
                continue;

            var templateItems = items.Select(i =>
            {
                var currency = currencyMap.GetValueOrDefault(i.CurrencyId);
                return new ProcessedRecurringItem
                {
                    Name = i.Name,
                    Description = i.Description,
                    IsIncome = i.TransactionType == TransactionTypes.Income,
                    Amount = i.Amount,
                    CurrencySymbol = currency?.Symbol ?? "",
                    IsSymbolRight = currency?.IsSymbolRight ?? true
                };
            }).ToList();

            var model = new RecurringTransactionsProcessedTemplateModel
            {
                UserName = recipient.DisplayName,
                ProcessedDate = processedDate,
                Items = templateItems
            };

            try
            {
                await mailSender.SendAsync(recipient, "RecurringTransactionsProcessedTemplate",
                    "RecurringTransactionsProcessed.Subject", model, cancellationToken: cancellationToken);
            }
            catch (Exception ex)
            {
                // Mail gönderimi hata verse bile job tamamlansın
                _logger.LogError(ex, "Recurring processed mail could not be sent for user {UserId}.", userId);
            }

            try
            {
                await pushSender.SendAsync(recipient, "RecurringTransactionsProcessed.Title", "RecurringTransactionsProcessed.Body",
                    new object[] { items.Count },
                    new Dictionary<string, string> { ["type"] = "RecurringTransactionsProcessed" },
                    cancellationToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Recurring processed push could not be sent for user {UserId}.", userId);
            }
        }
    }

    private async Task SendRecurringEndedNotificationEmailsAsync(
        IServiceProvider serviceProvider,
        Dictionary<int, List<(string Name, string Description, decimal Amount, int CurrencyId, TransactionTypes TransactionType)>> endedItemsByUser,
        DateTime endedDate,
        CancellationToken cancellationToken)
    {
        if (endedItemsByUser.Count == 0)
            return;

        var currencyRepository = serviceProvider.GetService<IRepository<Currency, ButceYonetDbContext>>();
        var recipientProvider = serviceProvider.GetRequiredService<INotificationRecipientProvider>();
        var mailSender = serviceProvider.GetRequiredService<IMailNotificationSender>();
        var pushSender = serviceProvider.GetRequiredService<IPushNotificationSender>();

        var currencyMap = await currencyRepository.GetAll()
            .ToDictionaryAsync(c => c.Id, cancellationToken);

        var recipients = await recipientProvider.GetManyAsync(endedItemsByUser.Keys, cancellationToken);

        foreach (var (userId, items) in endedItemsByUser)
        {
            if (items.Count == 0 || !recipients.TryGetValue(userId, out var recipient))
                continue;

            var endedIncomes = items
                .Where(i => i.TransactionType == TransactionTypes.Income)
                .Select(i => ToEndedItem(i, currencyMap))
                .ToList();

            var endedExpenses = items
                .Where(i => i.TransactionType == TransactionTypes.Expense)
                .Select(i => ToEndedItem(i, currencyMap))
                .ToList();

            var model = new RecurringTransactionEndedTemplateModel
            {
                UserName = recipient.DisplayName,
                EndedDate = endedDate,
                EndedIncomes = endedIncomes,
                EndedExpenses = endedExpenses
            };

            try
            {
                await mailSender.SendAsync(recipient, "RecurringTransactionEndedTemplate",
                    "RecurringTransactionEnded.Subject", model, cancellationToken: cancellationToken);
            }
            catch (Exception ex)
            {
                // Mail gönderimi hata verse bile job tamamlansın
                _logger.LogError(ex, "Recurring ended mail could not be sent for user {UserId}.", userId);
            }

            try
            {
                await pushSender.SendAsync(recipient, "RecurringTransactionEnded.Title", "RecurringTransactionEnded.Body",
                    new object[] { items.Count },
                    new Dictionary<string, string> { ["type"] = "RecurringTransactionEnded" },
                    cancellationToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Recurring ended push could not be sent for user {UserId}.", userId);
            }
        }
    }

    private static EndedRecurringItem ToEndedItem(
        (string Name, string Description, decimal Amount, int CurrencyId, TransactionTypes TransactionType) item,
        Dictionary<int, Currency> currencyMap)
    {
        var currency = currencyMap.GetValueOrDefault(item.CurrencyId);
        return new EndedRecurringItem
        {
            Name = item.Name,
            Description = item.Description,
            Amount = item.Amount,
            CurrencySymbol = currency?.Symbol ?? "",
            IsSymbolRight = currency?.IsSymbolRight ?? true
        };
    }
}
