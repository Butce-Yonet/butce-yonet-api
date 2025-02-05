using DotBoil.Entities;

namespace ButceYonet.Application.Domain.Entities;

public class TransactionLabel : BaseEntity
{
    public int NotebookLabelId { get; set; }
    public int TransactionId { get; set; }
    
    public virtual NotebookLabel NotebookLabel { get; set; }
    public virtual Transaction Transaction { get; set; }
}