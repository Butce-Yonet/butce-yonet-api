using ButceYonet.Application.Domain.Enums;
using DotBoil.Entities;

namespace ButceYonet.Application.Domain.Entities;

public class PlanFeature : BaseEntity
{
    public int PlanId { get; set; }
    public string Code { get; set; }
    public PlanFeatures Feature { get; set; }
    public int Count { get; set; }
    public string Description { get; set; }
    
    public virtual Plan Plan { get; set; }
}