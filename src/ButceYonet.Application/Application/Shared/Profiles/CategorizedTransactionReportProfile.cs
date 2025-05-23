using AutoMapper;
using ButceYonet.Application.Application.Shared.Dtos;
using ButceYonet.Application.Domain.Entities;

namespace ButceYonet.Application.Application.Shared.Profiles;

public class CategorizedTransactionReportProfile : Profile
{
    public CategorizedTransactionReportProfile()
    {
        CreateMap<CategorizedTransactionReport, CategorizedTransactionReportDto>()
            .ForMember(p => p.TransactionType, p => p.MapFrom(p => p.TransactionType))
            .ForMember(p => p.Amount, p => p.MapFrom(p => p.Currency))
            .ForMember(p => p.Term, p => p.MapFrom(p => p.Term));
    }
}