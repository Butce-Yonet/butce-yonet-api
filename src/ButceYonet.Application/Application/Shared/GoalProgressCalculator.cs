using ButceYonet.Application.Application.Shared.Dtos;

namespace ButceYonet.Application.Application.Shared;

/// <summary>
/// RemainingAmount/ProgressPercent/IsCompleted DB'de tutulmaz, TargetAmount/CurrentAmount'tan türetilir.
/// </summary>
public static class GoalProgressCalculator
{
    public static void Apply(GoalDto dto)
    {
        dto.RemainingAmount = Math.Max(0, dto.TargetAmount - dto.CurrentAmount);
        dto.ProgressPercent = dto.TargetAmount > 0
            ? Math.Min(100, Math.Round(dto.CurrentAmount / dto.TargetAmount * 100, 2))
            : 0;
        dto.IsCompleted = dto.CurrentAmount >= dto.TargetAmount;
    }
}
