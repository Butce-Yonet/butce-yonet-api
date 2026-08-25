using DotBoil.Entities;

namespace ButceYonet.Application.Domain.Entities;

public class SubscriptionLabel : BaseEntity
{
    public int SubscriptionId { get; set; }
    public int UserLabelId { get; set; }

    public virtual Subscription Subscription { get; set; }
    public virtual UserLabel UserLabel { get; set; }
}
