using DotBoil.Entities;

namespace ButceYonet.Application.Domain.Entities;

public class Goal : BaseEntity
{
    public int UserId { get; set; }
    public string Name { get; set; }
    public decimal TargetAmount { get; set; }
    public decimal CurrentAmount { get; set; }
    public int CurrencyId { get; set; }
    public DateTime? Deadline { get; set; }

    public virtual Currency Currency { get; set; }
    public virtual ICollection<GoalLabel> GoalLabels { get; set; }

    public Goal()
    {
        GoalLabels = new List<GoalLabel>();
    }
}
