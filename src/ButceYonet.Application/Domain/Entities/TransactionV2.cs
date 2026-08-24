using ButceYonet.Application.Domain.Enums;
using DotBoil.Entities;

namespace ButceYonet.Application.Domain.Entities;

public class TransactionV2 : BaseEntity
{
    public int NotebookV2Id { get; set; }
    public string ExternalId { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public decimal Amount { get; set; }
    public int CurrencyId { get; set; }
    public TransactionTypes TransactionType { get; set; }
    public bool IsMatched { get; set; }
    public bool IsProceed { get; set; }
    public DateTime TransactionDate { get; set; }

    public virtual NotebookV2 NotebookV2 { get; set; }
    public virtual Currency Currency { get; set; }
    public virtual ICollection<TransactionLabelV2> TransactionLabelsV2 { get; set; }

    public TransactionV2()
    {
        TransactionLabelsV2 = new List<TransactionLabelV2>();
    }
}
