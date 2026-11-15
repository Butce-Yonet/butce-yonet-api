using AutoMapper;
using ButceYonet.Application.Application.Interfaces;
using ButceYonet.Application.Application.Shared.Dtos;
using ButceYonet.Application.Domain.Entities;

namespace ButceYonet.Application.Application.Shared;

/// <summary>
/// Bütçe listesini, her biri kendi aktif dönemine göre hesaplanmış BudgetDto'lara çevirir.
/// Bütçeler farklı PeriodStartDay'lere sahip olabileceği için tüm dönemleri kapsayan aralık tek seferde yüklenir.
/// </summary>
public static class BudgetStatusBuilder
{
    public static async Task<List<BudgetDto>> BuildAsync(
        List<Budget> budgets,
        IBudgetSpendingCalculator spendingCalculator,
        IMapper mapper,
        int userId,
        DateTime asOf,
        CancellationToken cancellationToken)
    {
        if (budgets.Count == 0)
            return new List<BudgetDto>();

        var periods = budgets.ToDictionary(b => b.Id, b => BudgetPeriodCalculator.GetPeriod(b.PeriodStartDay, asOf));

        var snapshot = await spendingCalculator.LoadAsync(
            userId,
            periods.Values.Min(p => p.Start),
            periods.Values.Max(p => p.End),
            cancellationToken);

        return budgets
            .Select(budget =>
            {
                var (start, end) = periods[budget.Id];
                var dto = mapper.Map<BudgetDto>(budget);
                BudgetProgressCalculator.Apply(dto, snapshot.GetSpent(budget, start, end), start, end, asOf);
                return dto;
            })
            .ToList();
    }
}
