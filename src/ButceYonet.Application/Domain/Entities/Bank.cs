using DotBoil.Entities;

namespace ButceYonet.Application.Domain.Entities;

public class Bank : BaseEntity
{
    public string Bid { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public string TypeName { get; set; }

    public virtual ICollection<BankAccount> BankAccounts { get; set; }

    public Bank()
    {
        BankAccounts = new List<BankAccount>();
    }
}