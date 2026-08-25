using System.Text.Json;
using ButceYonet.Application.Application.Interfaces;
using ButceYonet.Application.Domain.Entities;
using ButceYonet.Application.Domain.Enums;
using ButceYonet.Application.Infrastructure.Data;
using DotBoil.EFCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace ButceYonet.Application.Infrastructure.Jobs;

// Transaction event'leri sadece "an"a bakan sinyaller üretebiliyor (bkz. TransactionCreatedDomainEventConsumer).
// Streak, sessizlik ve hedef-deadline sinyalleri ise bir zaman aralığına bakıyor, bu yüzden
// günlük çalışan ayrı bir job'la üretiliyor. Her üç kontrol de bağımsız, biri patlarsa diğerleri etkilenmez.
public class EngagementSignalJob : BackgroundService
{
    private static readonly int[] StreakThresholds = { 3, 7, 14, 30, 60, 100 };
    private const int StreakLookbackDays = 100;
    private const int InactivityThresholdDays = 3;
    private const int GoalDeadlineWindowDays = 7;

    private readonly ILogger<EngagementSignalJob> _logger;
    private readonly IServiceProvider _serviceProvider;

    public EngagementSignalJob(
        ILogger<EngagementSignalJob> logger,
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
                _logger.LogInformation("Engagement signal job timed out.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Engagement signal job failed.");
            }

            await Task.Delay(TimeSpan.FromHours(24), stoppingToken);
        }
    }

    private async Task RunAsync(CancellationToken cancellationToken)
    {
        using var scope = _serviceProvider.CreateAsyncScope();
        var today = DateTime.UtcNow.Date;

        await CreateStreakSignalsAsync(scope.ServiceProvider, today, cancellationToken);
        await CreateInactivitySignalsAsync(scope.ServiceProvider, today, cancellationToken);
        await CreateGoalDeadlineSignalsAsync(scope.ServiceProvider, today, cancellationToken);
    }

    // Kullanıcının ardışık kaç gündür işlem girdiğini (streak) hesaplar.
    // "Bugünkü streak" ile "dünkü streak" karşılaştırılır: eşik ilk defa bugün aşıldıysa
    // (dün altında, bugün üstünde/eşitse) sinyal üretilir — aynı streak için her gün tekrar üretmez.
    private static async Task CreateStreakSignalsAsync(IServiceProvider serviceProvider, DateTime today, CancellationToken cancellationToken)
    {
        var transactionRepository = serviceProvider.GetRequiredService<IRepository<TransactionV2, ButceYonetDbContext>>();
        var since = today.AddDays(-StreakLookbackDays);

        // CreateTime kullanılıyor (TransactionDate değil): kullanıcı geçmişe/ileriye tarihli işlem
        // girebiliyor, ama CreateTime uygulamayı fiilen o gün kullandığını gösteriyor.
        var activity = await transactionRepository
            .GetAll()
            .Where(t => t.CreateTime >= since)
            .Select(t => new { UserId = t.NotebookV2.UserId, ActiveDate = t.CreateTime.Date })
            .Distinct()
            .ToListAsync(cancellationToken);

        if (activity.Count == 0)
            return;

        var activeDatesByUser = activity
            .GroupBy(a => a.UserId)
            .ToDictionary(g => g.Key, g => g.Select(a => a.ActiveDate).ToHashSet());

        var signalRepository = serviceProvider.GetRequiredService<IRepository<EngagementSignal, ButceYonetDbContext>>();
        var hasChanges = false;

        foreach (var (userId, activeDates) in activeDatesByUser)
        {
            var oldStreak = ComputeStreak(activeDates, today.AddDays(-1));
            var newStreak = ComputeStreak(activeDates, today);

            foreach (var threshold in StreakThresholds)
            {
                if (oldStreak >= threshold || newStreak < threshold)
                    continue;

                await signalRepository.AddAsync(new EngagementSignal
                {
                    UserId = userId,
                    Type = EngagementSignalType.StreakMilestone,
                    OccurredAt = DateTime.UtcNow,
                    PayloadJson = JsonSerializer.Serialize(new StreakMilestoneSignalPayload { StreakDays = threshold })
                });
                hasChanges = true;
            }
        }

        if (hasChanges)
            await signalRepository.SaveChangesAsync();
    }

    // asOfDate'ten geriye doğru, aktif-gün kümesinde ilk boşluk bulana kadar sayar.
    // Örn. aktif günler {bugün, dün, evvelsi gün} ise asOfDate=bugün için streak=3 döner.
    private static int ComputeStreak(HashSet<DateTime> activeDates, DateTime asOfDate)
    {
        var streak = 0;
        var cursor = asOfDate;

        while (activeDates.Contains(cursor))
        {
            streak++;
            cursor = cursor.AddDays(-1);
        }

        return streak;
    }

    // Son işleminden bu yana 3+ gün geçen kullanıcılar için tek seferlik bir sessizlik uyarısı üretir.
    // "Tek seferlik" olması, aynı sessizlik döneminde (son işlem tarihi değişmediği sürece) her gün
    // yeniden üretilmemesi için — bunun kontrolü, o kullanıcı için son işlemden SONRAKİ bir
    // InactivityWarning zaten var mı diye bakılarak yapılıyor.
    private static async Task CreateInactivitySignalsAsync(IServiceProvider serviceProvider, DateTime today, CancellationToken cancellationToken)
    {
        var transactionRepository = serviceProvider.GetRequiredService<IRepository<TransactionV2, ButceYonetDbContext>>();
        var inactivityCutoff = today.AddDays(-InactivityThresholdDays);

        var lastActivityByUser = await transactionRepository
            .GetAll()
            .GroupBy(t => t.NotebookV2.UserId)
            .Select(g => new { UserId = g.Key, LastActivity = g.Max(t => t.CreateTime) })
            .Where(x => x.LastActivity <= inactivityCutoff)
            .ToListAsync(cancellationToken);

        if (lastActivityByUser.Count == 0)
            return;

        var signalRepository = serviceProvider.GetRequiredService<IRepository<EngagementSignal, ButceYonetDbContext>>();
        var hasChanges = false;

        foreach (var user in lastActivityByUser)
        {
            var alreadyWarned = await signalRepository
                .GetAll()
                .AnyAsync(s =>
                    s.UserId == user.UserId &&
                    s.Type == EngagementSignalType.InactivityWarning &&
                    s.OccurredAt > user.LastActivity, cancellationToken);

            if (alreadyWarned)
                continue;

            await signalRepository.AddAsync(new EngagementSignal
            {
                UserId = user.UserId,
                Type = EngagementSignalType.InactivityWarning,
                OccurredAt = DateTime.UtcNow,
                PayloadJson = JsonSerializer.Serialize(new InactivityWarningSignalPayload
                {
                    DaysSinceLastTransaction = (today - user.LastActivity.Date).Days
                })
            });
            hasChanges = true;
        }

        if (hasChanges)
            await signalRepository.SaveChangesAsync();
    }

    // Deadline'ına 7 gün veya daha az kalmış ve henüz tamamlanmamış hedefler için uyarı üretir.
    // Aynı hedef için son 7 gün içinde zaten bir uyarı üretilmişse tekrar üretmez (haftada bir).
    private static async Task CreateGoalDeadlineSignalsAsync(IServiceProvider serviceProvider, DateTime today, CancellationToken cancellationToken)
    {
        var goalRepository = serviceProvider.GetRequiredService<IRepository<Goal, ButceYonetDbContext>>();
        var windowEnd = today.AddDays(GoalDeadlineWindowDays);

        var candidates = await goalRepository
            .GetAll()
            .Where(g =>
                g.Deadline.HasValue &&
                g.Deadline.Value.Date <= windowEnd &&
                g.Deadline.Value.Date >= today &&
                g.CurrentAmount < g.TargetAmount)
            .ToListAsync(cancellationToken);

        if (candidates.Count == 0)
            return;

        var signalRepository = serviceProvider.GetRequiredService<IRepository<EngagementSignal, ButceYonetDbContext>>();
        var repeatWindowStart = today.AddDays(-GoalDeadlineWindowDays);
        var hasChanges = false;

        foreach (var goal in candidates)
        {
            var alreadySignaled = await signalRepository
                .GetAll()
                .AnyAsync(s =>
                    s.GoalId == goal.Id &&
                    s.Type == EngagementSignalType.GoalDeadlineApproaching &&
                    s.OccurredAt >= repeatWindowStart, cancellationToken);

            if (alreadySignaled)
                continue;

            await signalRepository.AddAsync(new EngagementSignal
            {
                UserId = goal.UserId,
                Type = EngagementSignalType.GoalDeadlineApproaching,
                GoalId = goal.Id,
                OccurredAt = DateTime.UtcNow,
                PayloadJson = JsonSerializer.Serialize(new GoalDeadlineApproachingSignalPayload
                {
                    GoalName = goal.Name,
                    DaysRemaining = (goal.Deadline!.Value.Date - today).Days,
                    RemainingAmount = goal.TargetAmount - goal.CurrentAmount,
                    CurrencyId = goal.CurrencyId
                })
            });
            hasChanges = true;
        }

        if (hasChanges)
            await signalRepository.SaveChangesAsync();
    }
}
