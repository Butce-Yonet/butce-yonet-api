using AutoMapper;
using ButceYonet.Application.Application.Shared.Dtos;
using ButceYonet.Application.Domain.Entities;

namespace ButceYonet.Application.Application.Shared.Profiles;

public class GoalProfile : Profile
{
    public GoalProfile()
    {
        CreateMap<Goal, GoalDto>()
            .ForMember(p => p.Id, p => p.MapFrom(p => p.Id))
            .ForMember(p => p.Name, p => p.MapFrom(p => p.Name))
            .ForMember(p => p.TargetAmount, p => p.MapFrom(p => p.TargetAmount))
            .ForMember(p => p.CurrentAmount, p => p.MapFrom(p => p.CurrentAmount))
            .ForMember(p => p.Currency, p => p.MapFrom(p => p.Currency))
            .ForMember(p => p.Deadline, p => p.MapFrom(p => p.Deadline))
            .ForMember(p => p.RemainingAmount, p => p.Ignore())
            .ForMember(p => p.ProgressPercent, p => p.Ignore())
            .ForMember(p => p.IsCompleted, p => p.Ignore())
            .ForMember(p => p.Labels, p => p.MapFrom(p =>
                p.GoalLabels.Where(gl => !gl.IsDeleted).Select(gl => gl.UserLabel)));
    }
}
