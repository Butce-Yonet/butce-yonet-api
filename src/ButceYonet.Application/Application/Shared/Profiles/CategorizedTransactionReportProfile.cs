using AutoMapper;
using ButceYonet.Application.Application.Shared.Dtos;
using ButceYonet.Application.Domain.Entities;

namespace ButceYonet.Application.Application.Shared.Profiles;

public class CategorizedTransactionReportProfile : Profile
{
    public CategorizedTransactionReportProfile()
    {
        CreateMap<CategorizedTransactionReport, CategorizedTransactionReportDto>()
            .ForMember(p => p.Notebook, p => p.MapFrom<NotebookResolver>())
            .ForMember(p => p.NotebookLabel, p => p.MapFrom<NotebookLabelResolver>())
            .ForMember(p => p.TransactionType, p => p.MapFrom(p => p.TransactionType))
            .ForMember(p => p.Currency, p => p.MapFrom<CurrencyResolver>())
            .ForMember(p => p.Amount, p => p.MapFrom(p => p.Amount))
            .ForMember(p => p.Term, p => p.MapFrom(p => p.Term));
    }

    public class NotebookResolver : IValueResolver<CategorizedTransactionReport, CategorizedTransactionReportDto, NotebookDto>
    {
        public NotebookDto Resolve(CategorizedTransactionReport source, CategorizedTransactionReportDto destination,
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

    public class NotebookLabelResolver : IValueResolver<CategorizedTransactionReport, CategorizedTransactionReportDto, NotebookLabelDto>
    {
        public NotebookLabelDto Resolve(CategorizedTransactionReport source, CategorizedTransactionReportDto destination,
            NotebookLabelDto destMember, ResolutionContext context)
        {
            if (source.NotebookLabel is null)
                return null;

            return new NotebookLabelDto
            {
                Id = source.NotebookLabel.Id,
                Name = source.NotebookLabel.Name,
                ColorCode = source.NotebookLabel.ColorCode
            };
        }
    }

    public class CurrencyResolver : IValueResolver<CategorizedTransactionReport, CategorizedTransactionReportDto, CurrencyDto>
    {
        public CurrencyDto Resolve(CategorizedTransactionReport source, CategorizedTransactionReportDto destination,
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
                IsSymbolRight = source.Currency.IsSymbolRight
            };
        }
    }
}