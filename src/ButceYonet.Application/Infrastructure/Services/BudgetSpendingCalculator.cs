using ButceYonet.Application.Application.Interfaces;
using ButceYonet.Application.Domain.Entities;
using ButceYonet.Application.Domain.Enums;
using ButceYonet.Application.Infrastructure.Data;
using DotBoil.EFCore;
using Microsoft.EntityFrameworkCore;

namespace ButceYonet.Application.Infrastructure.Services;

public class BudgetSpendingCalculator : IBudgetSpendingCalculator
{
    private readonly IRepository<CategorizedTransactionReportV2, ButceYonetDbContext> _categorizedRepository;
    private readonly IRepository<NonCategorizedTransactionReport, ButceYonetDbContext> _nonCategorizedRepository;

    public BudgetSpendingCalculator(
        IRepository<CategorizedTransactionReportV2, ButceYonetDbContext> categorizedRepository,
        IRepository<NonCategorizedTransactionReport, ButceYonetDbContext> nonCategorizedRepository)
    {
        _categorizedRepository = categorizedRepository;
        _nonCategorizedRepository = nonCategorizedRepository;
    }

    public async Task<IBudgetSpendingSnapshot> LoadAsync(int userId, DateTime from, DateTime to, CancellationToken cancellationToken = default)
    {
        var fromDate = from.Date;
        var toDate = to.Date;

        // Rapor tabloları günlük bucket tutuyor (Term = işlem gününün başı) ve bir kullanıcının aylık satır sayısı
        // küçük; aralığın tamamını grup bazında çekip dönem hesabını bellekte yapmak, bütçe başına sorgu atmaktan ucuz.
        var categorized = await _categorizedRepository
            .GetAll()
            .Where(r =>
                r.NotebookV2.UserId == userId &&
                r.TransactionType == TransactionTypes.Expense &&
                r.Term >= fromDate &&
                r.Term <= toDate)
            .GroupBy(r => new { r.UserLabelId, r.CurrencyId, r.Term })
            .Select(g => new SpendingRow(g.Key.UserLabelId, g.Key.CurrencyId, g.Key.Term, g.Sum(r => r.Amount)))
            .ToListAsync(cancellationToken);

        var nonCategorized = await _nonCategorizedRepository
            .GetAll()
            .Where(r =>
                r.NotebookV2.UserId == userId &&
                r.TransactionType == TransactionTypes.Expense &&
                r.Term >= fromDate &&
                r.Term <= toDate)
            .GroupBy(r => new { r.CurrencyId, r.Term })
            .Select(g => new SpendingRow(null, g.Key.CurrencyId, g.Key.Term, g.Sum(r => r.Amount)))
            .ToListAsync(cancellationToken);

        return new Snapshot(categorized, nonCategorized);
    }

    private record SpendingRow(int? UserLabelId, int CurrencyId, DateTime Term, decimal Amount);

    private class Snapshot : IBudgetSpendingSnapshot
    {
        private readonly List<SpendingRow> _categorized;
        private readonly List<SpendingRow> _nonCategorized;

        public Snapshot(List<SpendingRow> categorized, List<SpendingRow> nonCategorized)
        {
            _categorized = categorized;
            _nonCategorized = nonCategorized;
        }

        public decimal GetSpent(Budget budget, DateTime start, DateTime end)
        {
            var startDate = start.Date;
            var endDate = end.Date;

            var rows = budget.UserLabelId.HasValue
                ? _categorized.Where(r => r.UserLabelId == budget.UserLabelId)
                : _nonCategorized;

            return rows
                .Where(r => r.CurrencyId == budget.CurrencyId && r.Term >= startDate && r.Term <= endDate)
                .Sum(r => r.Amount);
        }
    }
}
