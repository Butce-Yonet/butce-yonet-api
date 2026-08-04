using AutoMapper;
using ButceYonet.Application.Application.Shared.Dtos;
using ButceYonet.Application.Domain.Entities;

namespace ButceYonet.Application.Application.Shared.Profiles;

public class TransactionLabelProfile : Profile
{
    public TransactionLabelProfile()
    {
        CreateMap<TransactionLabelV2, UserLabelDto>()
            .ForMember(p => p.Id, p => p.MapFrom(p => p.UserLabel.Id))
            .ForMember(p => p.Name, p => p.MapFrom(p => p.UserLabel.Name))
            .ForMember(p => p.ColorCode, p => p.MapFrom(p => p.UserLabel.ColorCode));
    }
}