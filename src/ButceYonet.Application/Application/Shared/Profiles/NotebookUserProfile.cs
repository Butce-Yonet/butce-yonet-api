using AutoMapper;
using ButceYonet.Application.Application.Shared.Dtos;
using ButceYonet.Application.Domain.Entities;

namespace ButceYonet.Application.Application.Shared.Profiles;

public class NotebookUserProfile : Profile
{
    public NotebookUserProfile()
    {
        CreateMap<NotebookUser, NotebookUserDto>()
            .ForMember(p => p.Id, m => m.MapFrom(m => m.Id))
            .ForMember(p => p.UserId, m => m.MapFrom(m => m.UserId))
            .ForMember(p => p.NotebookId, m => m.MapFrom(m => m.NotebookId))
            .ForMember(p => p.IsDefault, m => m.MapFrom(m => m.IsDefault));
    }
}