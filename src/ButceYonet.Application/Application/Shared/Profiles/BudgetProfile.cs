using AutoMapper;
using ButceYonet.Application.Application.Shared.Dtos;
using ButceYonet.Application.Domain.Entities;

namespace ButceYonet.Application.Application.Shared.Profiles;

public class BudgetProfile : Profile
{
    public BudgetProfile()
    {
        CreateMap<Budget, BudgetDto>()
            .ForMember(p => p.Id, p => p.MapFrom(p => p.Id))
            .ForMember(p => p.Amount, p => p.MapFrom(p => p.Amount))
            .ForMember(p => p.Currency, p => p.MapFrom(p => p.Currency))
            .ForMember(p => p.Label, p => p.MapFrom(p => p.UserLabel))
            .ForMember(p => p.IsGeneral, p => p.MapFrom(p => !p.UserLabelId.HasValue))
            .ForMember(p => p.PeriodStartDay, p => p.MapFrom(p => p.PeriodStartDay))
            .ForMember(p => p.PeriodStart, p => p.Ignore())
            .ForMember(p => p.PeriodEnd, p => p.Ignore())
            .ForMember(p => p.SpentAmount, p => p.Ignore())
            .ForMember(p => p.RemainingAmount, p => p.Ignore())
            .ForMember(p => p.OverAmount, p => p.Ignore())
            .ForMember(p => p.Percentage, p => p.Ignore())
            .ForMember(p => p.Status, p => p.Ignore())
            .ForMember(p => p.DaysLeft, p => p.Ignore())
            .ForMember(p => p.DailyAllowance, p => p.Ignore())
            .ForMember(p => p.History, p => p.Ignore());
    }
}
