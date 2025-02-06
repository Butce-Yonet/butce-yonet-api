using DotBoil.Entities;

namespace ButceYonet.Application.Domain.Entities;

public class UserPlan : BaseEntity
{
    public int UserId { get; set; }
    public int PlanId { get; set; }
    public DateTime? ExpirationDate { get; set; }
    
    public virtual Plan Plan { get; set; }
}