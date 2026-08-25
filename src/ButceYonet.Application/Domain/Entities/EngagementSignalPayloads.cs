namespace ButceYonet.Application.Domain.Entities;

public class GoalProgressSignalPayload
{
    public string GoalName { get; set; }
    public int Percentage { get; set; }
    public decimal TargetAmount { get; set; }
    public decimal CurrentAmount { get; set; }
    public int CurrencyId { get; set; }
}

public class GoalDeadlineApproachingSignalPayload
{
    public string GoalName { get; set; }
    public int DaysRemaining { get; set; }
    public decimal RemainingAmount { get; set; }
    public int CurrencyId { get; set; }
}

public class StreakMilestoneSignalPayload
{
    public int StreakDays { get; set; }
}

public class InactivityWarningSignalPayload
{
    public int DaysSinceLastTransaction { get; set; }
}

public class AnomalousSpendingSignalPayload
{
    public string TransactionName { get; set; }
    public decimal Amount { get; set; }
    public decimal AverageAmount { get; set; }
    public decimal Ratio { get; set; }
    public int CurrencyId { get; set; }
    public int CategoryLabelId { get; set; }
}
