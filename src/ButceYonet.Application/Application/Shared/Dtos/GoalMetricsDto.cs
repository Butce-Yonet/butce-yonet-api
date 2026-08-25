namespace ButceYonet.Application.Application.Shared.Dtos;

public class GoalMetricsDto
{
    public int ActiveGoalCount { get; set; }
    public int CompletedGoalCount { get; set; }
    public decimal TotalTargetAmount { get; set; }
    public decimal TotalSavedAmount { get; set; }
    public CurrencyDto Currency { get; set; }
}
