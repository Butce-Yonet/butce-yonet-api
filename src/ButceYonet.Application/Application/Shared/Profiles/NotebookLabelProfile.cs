using AutoMapper;
using ButceYonet.Application.Application.Shared.Dtos;
using ButceYonet.Application.Domain.Entities;

namespace ButceYonet.Application.Application.Shared.Profiles;

public class NotebookLabelProfile : Profile
{
    public NotebookLabelProfile()
    {
        CreateMap<NotebookLabel, NotebookLabelDto>()
            .ForMember(m => m.Id, m => m.MapFrom(m => m.Id))
            .ForMember(m => m.Name, m => m.MapFrom(m => m.Name))
            .ForMember(m => m.ColorCode, m => m.MapFrom(m => m.ColorCode));
    }
}