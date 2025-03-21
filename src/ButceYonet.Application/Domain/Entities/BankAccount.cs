using System.Collections;
using DotBoil.Entities;

namespace ButceYonet.Application.Domain.Entities;

public class BankAccount : BaseEntity
{
    public int NotebookId { get; set; }
    public int BankId { get; set; }
    public int UserId { get; set; }
    public string Description { get; set; }
    
    public virtual Notebook Notebook { get; set; }
    public virtual Bank Bank { get; set; }
    public virtual ICollection<Transaction> Transactions { get; set; }

    public BankAccount()
    {
        Transactions = new List<Transaction>();
    }
}