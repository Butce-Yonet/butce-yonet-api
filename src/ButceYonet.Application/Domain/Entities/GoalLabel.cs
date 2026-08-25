using DotBoil.Entities;

namespace ButceYonet.Application.Domain.Entities;

public class GoalLabel : BaseEntity
{
    public int GoalId { get; set; }
    public int UserLabelId { get; set; }

    public virtual Goal Goal { get; set; }
    public virtual UserLabel UserLabel { get; set; }
}
