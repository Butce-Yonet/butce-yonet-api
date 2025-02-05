using DotBoil.Entities;

namespace ButceYonet.Application.Domain.Entities;

public class NotebookUser : BaseEntity
{
    public int UserId { get; set; }
    public int NotebookId { get; set; }
    
    public virtual Notebook Notebook { get; set; }
}