using DotBoil.Entities;

namespace ButceYonet.Application.Domain.Entities;

public class Plan : BaseEntity
{
    public string Title { get; set; }
    public string Description { get; set; }
    public decimal Amount { get; set; }
    public bool IsDefault { get; set; }
    
    public virtual ICollection<PlanFeature> PlanFeatures { get; set; }
    public virtual ICollection<UserPlan> UserPlans { get; set; }

    public Plan()
    {
        PlanFeatures = new List<PlanFeature>();
    }
}