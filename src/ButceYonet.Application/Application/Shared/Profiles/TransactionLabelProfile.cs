using AutoMapper;
using ButceYonet.Application.Application.Shared.Dtos;
using ButceYonet.Application.Domain.Entities;

namespace ButceYonet.Application.Application.Shared.Profiles;

public class TransactionLabelProfile : Profile
{
    public TransactionLabelProfile()
    {
        CreateMap<TransactionLabel, NotebookLabelDto>()
            .ForMember(p => p.Id, p => p.MapFrom(p => p.NotebookLabel.Id))
            .ForMember(p => p.Name, p => p.MapFrom(p => p.NotebookLabel.Name))
            .ForMember(p => p.ColorCode, p => p.MapFrom(p => p.NotebookLabel.ColorCode));
    }
}