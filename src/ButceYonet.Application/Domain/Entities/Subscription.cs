using ButceYonet.Application.Domain.Enums;
using DotBoil.Entities;

namespace ButceYonet.Application.Domain.Entities;

public class Subscription : BaseEntity
{
    public int UserId { get; set; }
    public string Name { get; set; }
    public decimal? Amount { get; set; }
    public int? CurrencyId { get; set; }
    public DateTime StartDate { get; set; }
    public RecurringTransactionIntervals Frequency { get; set; }
    public int? Interval { get; set; }
    public DateTime? NextOccurrence { get; set; }
    public DateTime? LastPaidDate { get; set; }
    public decimal? LastPaidAmount { get; set; }

    public virtual Currency Currency { get; set; }
    public virtual ICollection<SubscriptionLabel> SubscriptionLabels { get; set; }

    public Subscription()
    {
        SubscriptionLabels = new List<SubscriptionLabel>();
    }
}
