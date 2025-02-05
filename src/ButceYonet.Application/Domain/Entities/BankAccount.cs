using DotBoil.Entities;

namespace ButceYonet.Application.Domain.Entities;

public class BankAccount : BaseEntity
{
    public int BankId { get; set; }
    public string Description { get; set; }
    
    public virtual Bank Bank { get; set; }
}