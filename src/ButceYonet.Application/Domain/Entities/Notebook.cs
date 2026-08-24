using System.Collections;
using DotBoil.Entities;

namespace ButceYonet.Application.Domain.Entities;

public class Notebook : BaseEntity
{
    public string Name { get; set; }
    public bool IsDefault { get; set; }
    
    public virtual ICollection<NotebookUser> NotebookUsers { get; set; } 
    public virtual ICollection<TransactionV2> Transactions { get; set; }

    public virtual ICollection<NonCategorizedTransactionReport> NonCategorizedTransactionReports { get; set; }

    public Notebook()
    {
        NotebookUsers = new List<NotebookUser>();
        Transactions = new List<TransactionV2>();
        NonCategorizedTransactionReports = new List<NonCategorizedTransactionReport>();
    }
}