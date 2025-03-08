using ButceYonet.Application.Domain.Enums;
using DotBoil.Entities;

namespace ButceYonet.Application.Domain.Entities;

public class CategorizedTransactionReport : BaseEntity
{
    public int NotebookId { get; set; }
    public int NotebookLabelId { get; set; }
    public TransactionTypes TransactionType { get; set; }
    public int CurrencyId { get; set; }
    public decimal Amount { get; set; }
    public DateTime Term { get; set; }
    
    public virtual Notebook Notebook { get; set; }
    public virtual NotebookLabel NotebookLabel { get; set; }
    public virtual Currency Currency { get; set; }
}