using DotBoil.Entities;

namespace ButceYonet.Application.Domain.Entities;

public class NotebookLabel : BaseEntity
{
    public int NotebookId { get; set; }
    public string Name { get; set; }
    public string ColorCode { get; set; }
    
    public virtual Notebook Notebook { get; set; }
    public virtual ICollection<TransactionLabel> TransactionLabels { get; set; }
    
    public virtual ICollection<CategorizedTransactionReport> CategorizedTransactionReports { get; set; }

    public NotebookLabel()
    {
        TransactionLabels = new List<TransactionLabel>();
        CategorizedTransactionReports = new List<CategorizedTransactionReport>();
    }
}