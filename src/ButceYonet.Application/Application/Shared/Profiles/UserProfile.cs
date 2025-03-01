using AutoMapper;
using ButceYonet.Application.Application.Shared.Dtos;
using ButceYonet.Application.Domain.Entities;

namespace ButceYonet.Application.Application.Shared.Profiles;

public class UserProfile : Profile
{
    public UserProfile()
    {
        CreateMap<User, UserDto>()
            .ForMember(p => p.Id, p => p.MapFrom(p => p.Id))
            .ForMember(p => p.Name, p => p.MapFrom(p => p.Name))
            .ForMember(p => p.Surname, p => p.MapFrom(p => p.Surname))
            .ForMember(p => p.Username, p => p.MapFrom(p => p.Username))
            .ForMember(p => p.Email, p => p.MapFrom(p => p.Email));
    }
}