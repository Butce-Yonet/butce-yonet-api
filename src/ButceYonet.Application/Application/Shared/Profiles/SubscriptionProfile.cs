using AutoMapper;
using ButceYonet.Application.Application.Shared.Dtos;
using ButceYonet.Application.Domain.Entities;

namespace ButceYonet.Application.Application.Shared.Profiles;

public class SubscriptionProfile : Profile
{
    public SubscriptionProfile()
    {
        CreateMap<Subscription, SubscriptionDto>()
            .ForMember(p => p.Id, p => p.MapFrom(p => p.Id))
            .ForMember(p => p.Name, p => p.MapFrom(p => p.Name))
            .ForMember(p => p.Amount, p => p.MapFrom(p => p.Amount))
            .ForMember(p => p.Currency, p => p.MapFrom(p => p.Currency))
            .ForMember(p => p.StartDate, p => p.MapFrom(p => p.StartDate))
            .ForMember(p => p.Frequency, p => p.MapFrom(p => p.Frequency))
            .ForMember(p => p.Interval, p => p.MapFrom(p => p.Interval))
            .ForMember(p => p.NextOccurrence, p => p.MapFrom(p => p.NextOccurrence))
            .ForMember(p => p.LastPaidDate, p => p.MapFrom(p => p.LastPaidDate))
            .ForMember(p => p.LastPaidAmount, p => p.MapFrom(p => p.LastPaidAmount))
            .ForMember(p => p.Status, p => p.Ignore())
            .ForMember(p => p.Labels, p => p.MapFrom(p =>
                p.SubscriptionLabels.Where(sl => !sl.IsDeleted).Select(sl => sl.UserLabel)));
    }
}
