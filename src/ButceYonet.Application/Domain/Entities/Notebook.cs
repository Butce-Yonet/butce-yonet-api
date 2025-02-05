using DotBoil.Entities;

namespace ButceYonet.Application.Domain.Entities;

public class Notebook : BaseEntity
{
    public string Name { get; set; }
    public bool IsDefault { get; set; }
    
    public virtual ICollection<NotebookUser> NotebookUsers { get; set; } 
    public virtual ICollection<NotebookLabel> NotebookLabels { get; set; }
    public virtual ICollection<Transaction> Transactions { get; set; }

    public Notebook()
    {
        NotebookUsers = new List<NotebookUser>();
        NotebookLabels = new List<NotebookLabel>();
        Transactions = new List<Transaction>();
    }
}