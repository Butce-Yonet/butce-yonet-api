using ButceYonet.Application.Domain.Entities;

namespace ButceYonet.Application.Application.Shared.Dtos;

public class PlanFeatureDto
{
    public PlanFeature Feature  { get; set; }
    public int Count { get; set; }
    public string Description { get; set; }
}