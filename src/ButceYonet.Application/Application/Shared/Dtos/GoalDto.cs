namespace ButceYonet.Application.Application.Shared.Dtos;

public class GoalDto
{
    public int Id { get; set; }
    public string Name { get; set; }
    public decimal TargetAmount { get; set; }
    public decimal CurrentAmount { get; set; }
    public decimal RemainingAmount { get; set; }
    public decimal ProgressPercent { get; set; }
    public bool IsCompleted { get; set; }
    public CurrencyDto Currency { get; set; }
    public DateTime? Deadline { get; set; }
    public List<UserLabelDto> Labels { get; set; } = new();
}
