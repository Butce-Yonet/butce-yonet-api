using AutoMapper;
using ButceYonet.Application.Application.Shared.Dtos;
using ButceYonet.Application.Domain.Entities;

namespace ButceYonet.Application.Application.Shared.Profiles;

public class TransactionProfile : Profile
{
    public TransactionProfile()
    {
        CreateMap<Transaction, TransactionDto>()
            .ForMember(p => p.Id, p => p.MapFrom(p => p.Id))
            .ForMember(p => p.NotebookId, p => p.MapFrom(p => p.NotebookId))
            .ForMember(p => p.Name, p => p.MapFrom(p => p.Name))
            .ForMember(p => p.Description, p => p.MapFrom(p => p.Description))
            .ForMember(p => p.Amount, p => p.MapFrom(p => p.Amount))
            .ForMember(p => p.TransactionType, p => p.MapFrom(p => p.TransactionType))
            .ForMember(p => p.IsMatched, p => p.MapFrom(p => p.IsMatched))
            .ForMember(p => p.IsProceed, p => p.MapFrom(p => p.IsProceed))
            .ForMember(p => p.TransactionDate, p => p.MapFrom(p => p.TransactionDate))
            .ForMember(p => p.Notebook, p => p.MapFrom(p => p.Notebook))
            .ForMember(p => p.Currency, p => p.MapFrom(p => p.Currency))
            .ForMember(p => p.Labels, p => p.MapFrom(p => p.TransactionLabels));
    }
}