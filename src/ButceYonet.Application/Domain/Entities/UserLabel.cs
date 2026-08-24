using DotBoil.Entities;

namespace ButceYonet.Application.Domain.Entities;

public class UserLabel : BaseEntity
{
    public int? UserId { get; set; }
    public string Name { get; set; }
    public string ColorCode { get; set; }

    public virtual ICollection<TransactionLabelV2> TransactionLabelsV2 { get; set; }
    public virtual ICollection<CategorizedTransactionReportV2> CategorizedTransactionReportV2s { get; set; }

    public UserLabel()
    {
        TransactionLabelsV2 = new List<TransactionLabelV2>();
        CategorizedTransactionReportV2s = new List<CategorizedTransactionReportV2>();
    }
}