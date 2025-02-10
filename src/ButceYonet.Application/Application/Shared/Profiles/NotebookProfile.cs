using AutoMapper;
using ButceYonet.Application.Application.Shared.Dtos;
using ButceYonet.Application.Domain.Entities;

namespace ButceYonet.Application.Application.Shared.Profiles;

public class NotebookProfile : Profile
{
    public NotebookProfile()
    {
        CreateMap<Notebook, NotebookDto>()
            .ForMember(m => m.Id, m => m.MapFrom(m => m.Id))
            .ForMember(m => m.Name, m => m.MapFrom(m => m.Name))
            .ForMember(m => m.IsDefault, m => m.MapFrom(m => m.IsDefault));
    }
}