using ButceYonet.Application.Application.Shared.Dtos;
using ButceYonet.Application.Domain.Enums;

namespace ButceYonet.Application.Application.Shared;

/// <summary>
/// BudgetDto'nun aktif dönem alanlarını (harcama, kalan, yüzde, durum, günlük harcanabilir) doldurur.
/// </summary>
public static class BudgetProgressCalculator
{
    public const int WarningThreshold = 80;
    public const int ExceededThreshold = 100;

    public static void Apply(BudgetDto dto, decimal spent, DateTime periodStart, DateTime periodEnd, DateTime asOf)
    {
        dto.PeriodStart = periodStart;
        dto.PeriodEnd = periodEnd;
        dto.SpentAmount = spent;
        dto.RemainingAmount = Math.Max(0, dto.Amount - spent);
        dto.OverAmount = Math.Max(0, spent - dto.Amount);
        dto.Percentage = GetPercentage(spent, dto.Amount);
        dto.Status = GetStatus(dto.Percentage);
        dto.DaysLeft = BudgetPeriodCalculator.GetDaysLeft(periodEnd, asOf);
        dto.DailyAllowance = dto.DaysLeft > 0
            ? Math.Round(dto.RemainingAmount / dto.DaysLeft, 2)
            : 0;
    }

    public static decimal GetPercentage(decimal spent, decimal amount)
    {
        return amount > 0 ? Math.Round(spent / amount * 100, 2) : 0;
    }

    public static BudgetStatus GetStatus(decimal percentage)
    {
        if (percentage >= ExceededThreshold)
            return BudgetStatus.Exceeded;

        return percentage >= WarningThreshold ? BudgetStatus.Warning : BudgetStatus.OnTrack;
    }
}
