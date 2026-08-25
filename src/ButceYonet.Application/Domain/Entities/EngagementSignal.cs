using ButceYonet.Application.Domain.Enums;
using DotBoil.Entities;

namespace ButceYonet.Application.Domain.Entities;

public class EngagementSignal : BaseEntity
{
    public int UserId { get; set; }
    public EngagementSignalType Type { get; set; }
    public int? GoalId { get; set; }
    public int? TransactionId { get; set; }
    public DateTime OccurredAt { get; set; }
    public string PayloadJson { get; set; }
    public bool IsSent { get; set; }
    public DateTime? SentAt { get; set; }

    public virtual Goal Goal { get; set; }
    public virtual TransactionV2 Transaction { get; set; }
}
