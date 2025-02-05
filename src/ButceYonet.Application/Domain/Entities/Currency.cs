using DotBoil.Entities;

namespace ButceYonet.Application.Domain.Entities;

public class Currency : BaseEntity
{
    public string Code { get; set; }
    public string Name { get; set; }
    public string Symbol { get; set; }
    public bool IsSymbolRight { get; set; }
    
    public virtual ICollection<Transaction> Transactions { get; set; }
}