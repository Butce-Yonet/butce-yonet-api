using AutoMapper;
using ButceYonet.Application.Application.Shared.Dtos;
using ButceYonet.Application.Domain.Entities;

namespace ButceYonet.Application.Application.Shared.Profiles;

public class PlanProfile : Profile
{
    public PlanProfile()
    {
        CreateMap<Plan, PlanDto>()
            .ForMember(p => p.Id, p => p.MapFrom(p => p.Id))
            .ForMember(p => p.Title, p => p.MapFrom(p => p.Title))
            .ForMember(p => p.Description, p => p.MapFrom(p => p.Description))
            .ForMember(p => p.Amount, p => p.MapFrom(p => p.Amount));

        CreateMap<PlanFeature, PlanFeatureDto>()
            .ForMember(p => p.Feature, p => p.MapFrom(p => p.Feature))
            .ForMember(p => p.Count, p => p.MapFrom(p => p.Count))
            .ForMember(p => p.Description, p => p.MapFrom(p => p.Description));
    }
}