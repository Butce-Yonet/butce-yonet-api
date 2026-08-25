using System.Text.Json;
using ButceYonet.Application.Application.Interfaces;
using ButceYonet.Application.Domain.Entities;
using ButceYonet.Application.Domain.Enums;
using ButceYonet.Application.Domain.Events;
using ButceYonet.Application.Infrastructure.Data;
using DotBoil.EFCore;
using DotBoil.MassTransit.Attributes;
using DotBoil.MassTransit.Consumers;
using MassTransit;
using Microsoft.EntityFrameworkCore;

namespace ButceYonet.Consumers;

[Consumer("transaction-created")]
public class TransactionCreatedDomainEventConsumer : BaseConsumer<TransactionCreatedDomainEvent>
{
    private static readonly int[] GoalProgressThresholds = { 50, 80, 100 };
    private const int AnomalyLookback = 10;
    private const int AnomalyMinSamples = 3;
    private const decimal AnomalyRatio = 1.75m;

    private readonly IServiceProvider _serviceProvider;
    private readonly ITransactionReportSyncService _reportSyncService;

    public TransactionCreatedDomainEventConsumer(
        IServiceProvider serviceProvider,
        ITransactionReportSyncService reportSyncService) : base(serviceProvider)
    {
        _serviceProvider = serviceProvider;
        _reportSyncService = reportSyncService;
    }

    public override async Task ConsumeEvent(ConsumeContext<TransactionCreatedDomainEvent> context)
    {
        // Goal katkıları IsMatched=false olabilir (etiketsiz hedef), bu yüzden
        // sinyal üretimi rapor senkron guard'ından önce ve bağımsız çalışır.
        await TryCreateGoalSignalsAsync(context.Message.Transaction);

        if (!context.Message.Transaction.IsMatched)
            return;

        using var scope = _serviceProvider.CreateScope();
        var transactionRepository = scope.ServiceProvider
            .GetRequiredService<IRepository<TransactionV2, ButceYonetDbContext>>();

        var transaction = await
            transactionRepository
                .Get()
                .Where(t => t.Id == context.Message.Transaction.Id)
                .Include(t => t.TransactionLabelsV2)
                .Include(t => t.NotebookV2)
                .FirstOrDefaultAsync();

        if (transaction is null)
            return;

        await Task.WhenAll(
            _reportSyncService.SyncNonCategorizedAsync(context.Message.Transaction, context.Message.Transaction.Amount),
            _reportSyncService.SyncCategorizedAsync(context.Message.Transaction, context.Message.Transaction.Amount));

        await TryCreateAnomalySignalAsync(transaction);

        transaction.IsProceed = true;
        transactionRepository.Update(transaction);
        await transactionRepository.SaveChangesAsync();
    }

    private async Task TryCreateGoalSignalsAsync(TransactionV2 transaction)
    {
        if (!transaction.GoalId.HasValue)
            return;

        using var scope = _serviceProvider.CreateScope();
        var goalRepository = scope.ServiceProvider.GetRequiredService<IRepository<Goal, ButceYonetDbContext>>();

        var goal = await goalRepository
            .Get()
            .Where(g => g.Id == transaction.GoalId.Value)
            .FirstOrDefaultAsync();

        if (goal is null || goal.TargetAmount <= 0)
            return;

        var newPercentage = goal.CurrentAmount / goal.TargetAmount * 100m;
        var oldPercentage = (goal.CurrentAmount - transaction.Amount) / goal.TargetAmount * 100m;

        var crossedThresholds = GoalProgressThresholds
            .Where(threshold => oldPercentage < threshold && newPercentage >= threshold)
            .ToList();

        if (crossedThresholds.Count == 0)
            return;

        var signalRepository = scope.ServiceProvider.GetRequiredService<IRepository<EngagementSignal, ButceYonetDbContext>>();

        foreach (var threshold in crossedThresholds)
        {
            var signalType = threshold == 100
                ? EngagementSignalType.GoalCompleted
                : EngagementSignalType.GoalProgressMilestone;

            var payload = JsonSerializer.Serialize(new GoalProgressSignalPayload
            {
                GoalName = goal.Name,
                Percentage = threshold,
                TargetAmount = goal.TargetAmount,
                CurrentAmount = goal.CurrentAmount,
                CurrencyId = goal.CurrencyId
            });

            await signalRepository.AddAsync(new EngagementSignal
            {
                UserId = goal.UserId,
                Type = signalType,
                GoalId = goal.Id,
                TransactionId = transaction.Id,
                OccurredAt = DateTime.UtcNow,
                PayloadJson = payload
            });
        }

        await signalRepository.SaveChangesAsync();
    }

    private async Task TryCreateAnomalySignalAsync(TransactionV2 transaction)
    {
        if (transaction.TransactionType != TransactionTypes.Expense)
            return;

        var categoryLabelId = transaction.TransactionLabelsV2.Select(l => l.UserLabelId).FirstOrDefault();
        if (categoryLabelId == 0)
            return;

        var userId = transaction.NotebookV2?.UserId ?? 0;
        if (userId == 0)
            return;

        using var scope = _serviceProvider.CreateScope();
        var transactionRepository = scope.ServiceProvider.GetRequiredService<IRepository<TransactionV2, ButceYonetDbContext>>();

        var recentAmounts = await transactionRepository
            .Get()
            .Where(t =>
                t.Id != transaction.Id &&
                t.TransactionType == TransactionTypes.Expense &&
                t.NotebookV2.UserId == userId &&
                t.TransactionLabelsV2.Any(l => l.UserLabelId == categoryLabelId))
            .OrderByDescending(t => t.TransactionDate)
            .Take(AnomalyLookback)
            .Select(t => t.Amount)
            .ToListAsync();

        if (recentAmounts.Count < AnomalyMinSamples)
            return;

        var average = recentAmounts.Average();
        if (average <= 0 || transaction.Amount < average * AnomalyRatio)
            return;

        var payload = JsonSerializer.Serialize(new AnomalousSpendingSignalPayload
        {
            TransactionName = transaction.Name,
            Amount = transaction.Amount,
            AverageAmount = average,
            Ratio = transaction.Amount / average,
            CurrencyId = transaction.CurrencyId,
            CategoryLabelId = categoryLabelId
        });

        var signalRepository = scope.ServiceProvider.GetRequiredService<IRepository<EngagementSignal, ButceYonetDbContext>>();

        await signalRepository.AddAsync(new EngagementSignal
        {
            UserId = userId,
            Type = EngagementSignalType.AnomalousSpending,
            TransactionId = transaction.Id,
            OccurredAt = DateTime.UtcNow,
            PayloadJson = payload
        });

        await signalRepository.SaveChangesAsync();
    }
}
