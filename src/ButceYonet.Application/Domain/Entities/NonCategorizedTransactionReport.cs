using ButceYonet.Application.Domain.Enums;
using DotBoil.Entities;

namespace ButceYonet.Application.Domain.Entities;

public class NonCategorizedTransactionReport : BaseEntity
{
    public int NotebookV2Id { get; set; }
    public TransactionTypes TransactionType { get; set; }
    public int CurrencyId { get; set; }
    public decimal Amount { get; set; }
    public DateTime Term { get; set; }

    public virtual NotebookV2 NotebookV2 { get; set; }
    public virtual Currency Currency { get; set; }
}
