using DotBoil.Entities;

namespace ButceYonet.Application.Domain.Entities;

public class Notebook : BaseEntity
{
    public string Name { get; set; }
    public bool IsDefault { get; set; }
    
    public virtual ICollection<NotebookUser> NotebookUsers { get; set; } 
}