using DotBoil.Entities;

namespace ButceYonet.Application.Domain.Entities;

public class TransactionLabelV2 : BaseEntity
{
    public int UserLabelId { get; set; }
    public int TransactionId { get; set; }
    public int? TransactionV2Id { get; set; }

    public virtual UserLabel UserLabel { get; set; }
    public virtual Transaction Transaction { get; set; }
    public virtual TransactionV2 TransactionV2 { get; set; }
}
