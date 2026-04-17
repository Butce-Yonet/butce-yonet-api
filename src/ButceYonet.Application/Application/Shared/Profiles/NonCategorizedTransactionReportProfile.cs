using AutoMapper;
using ButceYonet.Application.Application.Shared.Dtos;
using ButceYonet.Application.Domain.Entities;

namespace ButceYonet.Application.Application.Shared.Profiles;

public class NonCategorizedTransactionReportProfile : Profile
{
    public NonCategorizedTransactionReportProfile()
    {
        CreateMap<NonCategorizedTransactionReport, NonCategorizedTransactionReportDto>()
            .ForMember(p => p.NotebookDto, p => p.MapFrom<NotebookResolver>())
            .ForMember(p => p.TransactionTypes, p => p.MapFrom(p => p.TransactionType))
            .ForMember(p => p.Currency, p => p.MapFrom<CurrencyResolver>())
            .ForMember(p => p.Amount, p => p.MapFrom(p => p.Amount))
            .ForMember(p => p.Term, p => p.MapFrom(p => p.Term));
    }

    public class NotebookResolver : IValueResolver<NonCategorizedTransactionReport, NonCategorizedTransactionReportDto, NotebookDto>
    {
        public NotebookDto Resolve(NonCategorizedTransactionReport source, NonCategorizedTransactionReportDto destination,
            NotebookDto destMember, ResolutionContext context)
        {
            if (source.Notebook is null)
                return null;

            return new NotebookDto
            {
                Id = source.Notebook.Id,
                Name = source.Notebook.Name,
                IsDefault = source.Notebook.IsDefault
            };
        }
    }

    public class CurrencyResolver : IValueResolver<NonCategorizedTransactionReport, NonCategorizedTransactionReportDto, CurrencyDto>
    {
        public CurrencyDto Resolve(NonCategorizedTransactionReport source, NonCategorizedTransactionReportDto destination,
            CurrencyDto destMember, ResolutionContext context)
        {
            if (source.Currency is null)
                return null;

            return new CurrencyDto
            {
                Id = source.Currency.Id,
                Code = source.Currency.Code,
                Name = source.Currency.Name,
                Symbol = source.Currency.Symbol,
                IsSymbolRight = source.Currency.IsSymbolRight,
                Rank = source.Currency.Rank
            };
        }
    }
}