using ButceYonet.Application.Domain.Enums;
using DotBoil.Entities;

namespace ButceYonet.Application.Domain.Entities;

public class Transaction : BaseEntity
{
    public int? NotebookId { get; set; }
    public int? BankAccountId { get; set; }
    public string ExternalId { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public decimal Amount { get; set; }
    public int CurrencyId { get; set; }
    public TransactionTypes TransactionType { get; set; }
    public bool IsMatched { get; set; }
    public bool IsProceed { get; set; }
    public DateTime TransactionDate { get; set; }
    
    public virtual Notebook Notebook { get; set; }
    public virtual BankAccount BankAccount { get; set; }
    public virtual Currency Currency { get; set; }
    public virtual ICollection<TransactionLabel> TransactionLabels { get; set; }

    public Transaction()
    {
        TransactionLabels = new List<TransactionLabel>();
    }
}