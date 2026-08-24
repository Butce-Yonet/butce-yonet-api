using DotBoil.Entities;

namespace ButceYonet.Application.Domain.Entities;

public class NotebookV2 : BaseEntity
{
    public int UserId { get; set; }
    public string Name { get; set; }
    public DateTime TermStart { get; set; }
    public DateTime TermEnd { get; set; }

    public virtual ICollection<TransactionV2> Transactions { get; set; }
    public virtual ICollection<NonCategorizedTransactionReport> NonCategorizedTransactionReports { get; set; }

    public NotebookV2()
    {
        Transactions = new List<TransactionV2>();
        NonCategorizedTransactionReports = new List<NonCategorizedTransactionReport>();
    }
}
