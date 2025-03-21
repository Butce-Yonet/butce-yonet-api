using DotBoil.Entities;

namespace ButceYonet.Application.Domain.Entities;

public class Currency : BaseEntity
{
    public string Code { get; set; }
    public string Name { get; set; }
    public string Symbol { get; set; }
    public bool IsSymbolRight { get; set; }
    
    public virtual ICollection<Transaction> Transactions { get; set; }
    public virtual ICollection<CategorizedTransactionReport> CategorizedTransactionReports { get; set; }
    public virtual ICollection<NonCategorizedTransactionReport> NonCategorizedTransactionReports { get; set; }

    public Currency()
    {
        Transactions = new List<Transaction>();
        CategorizedTransactionReports = new List<CategorizedTransactionReport>();
        NonCategorizedTransactionReports = new List<NonCategorizedTransactionReport>();
    }
}