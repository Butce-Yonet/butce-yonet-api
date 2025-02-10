using AutoMapper;
using ButceYonet.Application.Application.Shared.Dtos;
using ButceYonet.Application.Domain.Entities;

namespace ButceYonet.Application.Application.Shared.Profiles;

public class CurrencyProfile : Profile
{
    public CurrencyProfile()
    {
        CreateMap<Currency, CurrencyDto>()
            .ForMember(p => p.Id, p => p.MapFrom(p => p.Id))
            .ForMember(p => p.Code, p => p.MapFrom(p => p.Code))
            .ForMember(p => p.Name, p => p.MapFrom(p => p.Name))
            .ForMember(p => p.Symbol, p => p.MapFrom(p => p.Symbol))
            .ForMember(p => p.IsSymbolRight, p => p.MapFrom(p => p.IsSymbolRight));
    }
}