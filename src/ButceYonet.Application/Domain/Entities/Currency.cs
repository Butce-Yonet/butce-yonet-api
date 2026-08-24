using DotBoil.Entities;

namespace ButceYonet.Application.Domain.Entities;

public class Currency : BaseEntity
{
    public string Code { get; set; }
    public string Name { get; set; }
    public string Symbol { get; set; }
    public bool IsSymbolRight { get; set; }
    public int Rank { get; set; }
    
    public virtual ICollection<TransactionV2> Transactions { get; set; }
    public virtual ICollection<NonCategorizedTransactionReport> NonCategorizedTransactionReports { get; set; }

    public Currency()
    {
        Transactions = new List<TransactionV2>();
        NonCategorizedTransactionReports = new List<NonCategorizedTransactionReport>();
    }
}