using System.Text.Json;
using ButceYonet.Application.Application.Interfaces;
using ButceYonet.Application.Application.Shared;
using ButceYonet.Application.Domain.Entities;
using ButceYonet.Application.Domain.Enums;
using ButceYonet.Application.Infrastructure.Data;
using DotBoil.EFCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace ButceYonet.Application.Infrastructure.Services;

public class BudgetSignalService : IBudgetSignalService
{
    private readonly ILogger<BudgetSignalService> _logger;
    private readonly IRepository<Budget, ButceYonetDbContext> _budgetRepository;
    private readonly IRepository<NotebookV2, ButceYonetDbContext> _notebookRepository;
    private readonly IRepository<EngagementSignal, ButceYonetDbContext> _signalRepository;
    private readonly IBudgetSpendingCalculator _spendingCalculator;

    public BudgetSignalService(
        ILogger<BudgetSignalService> logger,
        IRepository<Budget, ButceYonetDbContext> budgetRepository,
        IRepository<NotebookV2, ButceYonetDbContext> notebookRepository,
        IRepository<EngagementSignal, ButceYonetDbContext> signalRepository,
        IBudgetSpendingCalculator spendingCalculator)
    {
        _logger = logger;
        _budgetRepository = budgetRepository;
        _notebookRepository = notebookRepository;
        _signalRepository = signalRepository;
        _spendingCalculator = spendingCalculator;
    }

    public async Task EvaluateAsync(TransactionV2 transaction, TransactionV2 previousTransaction = null, CancellationToken cancellationToken = default)
    {
        try
        {
            await EvaluateInternalAsync(transaction, previousTransaction, cancellationToken);
        }
        catch (Exception ex) when (ex is not OperationCanceledException)
        {
            _logger.LogError(ex, "Budget signals could not be evaluated for transaction {TransactionId}.", transaction?.Id);
        }
    }

    private async Task EvaluateInternalAsync(TransactionV2 transaction, TransactionV2 previousTransaction, CancellationToken cancellationToken)
    {
        if (transaction is null || transaction.IsDeleted || transaction.TransactionType != TransactionTypes.Expense)
            return;

        var userId = transaction.NotebookV2?.UserId ?? await _notebookRepository
            .GetAll()
            .Where(n => n.Id == transaction.NotebookV2Id)
            .Select(n => n.UserId)
            .FirstOrDefaultAsync(cancellationToken);

        if (userId == 0)
            return;

        var labelIds = ActiveLabelIds(transaction);

        var budgets = await _budgetRepository
            .GetAll()
            .Where(b =>
                b.UserId == userId &&
                b.CurrencyId == transaction.CurrencyId &&
                (b.UserLabelId == null || labelIds.Contains(b.UserLabelId.Value)))
            .Include(b => b.UserLabel)
            .ToListAsync(cancellationToken);

        var now = DateTime.Now;

        // Geçmiş (ya da gelecek) döneme tarihli bir işlem için "bütçeni aştın" bildirimi anlamsız:
        // sadece işlem tarihi bütçenin AKTİF dönemine düşüyorsa değerlendir.
        var candidates = budgets
            .Select(b => (Budget: b, Period: BudgetPeriodCalculator.GetPeriod(b.PeriodStartDay, now)))
            .Where(x => transaction.TransactionDate.Date >= x.Period.Start && transaction.TransactionDate.Date <= x.Period.End)
            .ToList();

        if (candidates.Count == 0)
            return;

        var snapshot = await _spendingCalculator.LoadAsync(
            userId,
            candidates.Min(x => x.Period.Start),
            candidates.Max(x => x.Period.End),
            cancellationToken);

        var hasChanges = false;

        foreach (var (budget, period) in candidates)
        {
            var newSpent = snapshot.GetSpent(budget, period.Start, period.End);
            var oldSpent = newSpent
                           - Contribution(transaction, budget, period)
                           + Contribution(previousTransaction, budget, period);

            var signalType = CrossedSignal(
                BudgetProgressCalculator.GetPercentage(oldSpent, budget.Amount),
                BudgetProgressCalculator.GetPercentage(newSpent, budget.Amount));

            if (signalType is null)
                continue;

            // Aynı dönemde aynı eşik için tek bildirim: kullanıcı işlemi silip tekrar eklerse ya da
            // tutarı düşürüp tekrar artırırsa ikinci kez bildirim gitmez.
            var alreadySignaled = await _signalRepository
                .GetAll()
                .AnyAsync(s =>
                    s.BudgetId == budget.Id &&
                    s.Type == signalType.Value &&
                    s.OccurredAt >= period.Start, cancellationToken);

            if (alreadySignaled)
                continue;

            await _signalRepository.AddAsync(new EngagementSignal
            {
                UserId = userId,
                Type = signalType.Value,
                BudgetId = budget.Id,
                TransactionId = transaction.Id,
                OccurredAt = DateTime.UtcNow,
                PayloadJson = JsonSerializer.Serialize(new BudgetSignalPayload
                {
                    LabelName = budget.UserLabel?.Name,
                    Percentage = signalType == EngagementSignalType.BudgetExceeded
                        ? BudgetProgressCalculator.ExceededThreshold
                        : BudgetProgressCalculator.WarningThreshold,
                    BudgetAmount = budget.Amount,
                    SpentAmount = newSpent,
                    CurrencyId = budget.CurrencyId,
                    PeriodStart = period.Start
                })
            });
            hasChanges = true;
        }

        if (hasChanges)
            await _signalRepository.SaveChangesAsync();
    }

    /// <summary>
    /// Tek bir işlemle hem %80 hem %100 aşıldıysa (örn. %70 → %105) sadece BudgetExceeded üretilir;
    /// aynı anda iki bildirim göndermek gürültü.
    /// </summary>
    private static EngagementSignalType? CrossedSignal(decimal oldPercentage, decimal newPercentage)
    {
        if (oldPercentage < BudgetProgressCalculator.ExceededThreshold && newPercentage >= BudgetProgressCalculator.ExceededThreshold)
            return EngagementSignalType.BudgetExceeded;

        if (oldPercentage < BudgetProgressCalculator.WarningThreshold && newPercentage >= BudgetProgressCalculator.WarningThreshold)
            return EngagementSignalType.BudgetThresholdReached;

        return null;
    }

    /// <summary>
    /// İşlemin, bütçenin verilen dönemdeki harcamasına katkısı (rapor senkronuyla aynı kurallar).
    /// </summary>
    private static decimal Contribution(TransactionV2 transaction, Budget budget, (DateTime Start, DateTime End) period)
    {
        if (transaction is null ||
            transaction.IsDeleted ||
            transaction.TransactionType != TransactionTypes.Expense ||
            transaction.CurrencyId != budget.CurrencyId)
            return 0;

        var date = transaction.TransactionDate.Date;
        if (date < period.Start || date > period.End)
            return 0;

        if (budget.UserLabelId.HasValue && !ActiveLabelIds(transaction).Contains(budget.UserLabelId.Value))
            return 0;

        return transaction.Amount;
    }

    private static List<int> ActiveLabelIds(TransactionV2 transaction)
    {
        return (transaction.TransactionLabelsV2 ?? new List<TransactionLabelV2>())
            .Where(tl => !tl.IsDeleted)
            .Select(tl => tl.UserLabelId)
            .ToList();
    }
}
