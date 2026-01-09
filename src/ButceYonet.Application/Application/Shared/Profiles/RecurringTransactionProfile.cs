using System.Text.Json;
using AutoMapper;
using ButceYonet.Application.Application.Shared.Dtos;
using ButceYonet.Application.Domain.Entities;

namespace ButceYonet.Application.Application.Shared.Profiles;

public class RecurringTransactionProfile : Profile
{
    public RecurringTransactionProfile()
    {
        CreateMap<RecurringTransaction, RecurringTransactionDto>()
            .ForMember(p => p.Id, p => p.MapFrom(p => p.Id))
            .ForMember(p => p.Notebook, p => p.MapFrom<NotebookResolver>())
            .ForMember(p => p.Name, p => p.MapFrom(p => p.Name))
            .ForMember(p => p.Description, p => p.MapFrom(p => p.Description))
            .ForMember(p => p.StartDate, p => p.MapFrom(p => p.StartDate))
            .ForMember(p => p.EndDate, p => p.MapFrom(p => p.EndDate))
            .ForMember(p => p.Frequency, p => p.MapFrom(p => p.Frequency))
            .ForMember(p => p.Interval, p => p.MapFrom(p => p.Interval))
            .ForMember(p => p.NextOccurrence, p => p.MapFrom(p => p.NextOccurrence))
            .ForMember(p => p.Transaction, p => p.MapFrom<TransactionResolver>());
    }

    class NotebookResolver : IValueResolver<RecurringTransaction, RecurringTransactionDto, NotebookDto>
    {
        public NotebookDto Resolve(RecurringTransaction source, RecurringTransactionDto destination,
            NotebookDto destMember,
            ResolutionContext context)
        {
            if (context.Items.TryGetValue("Notebook", out object notebookObj))
            {
                var notebook = (Notebook)notebookObj;

                if (notebook is null)
                    return null;

                return new NotebookDto
                {
                    Id = notebook.Id,
                    IsDefault = notebook.IsDefault,
                    Name = notebook.Name
                };
            }

            return null;
        }
    }

    class TransactionResolver : IValueResolver<RecurringTransaction, RecurringTransactionDto, TransactionDto>
    {
        public TransactionDto Resolve(RecurringTransaction source, RecurringTransactionDto destination,
            TransactionDto destMember,
            ResolutionContext context)
        {
            Notebook notebook = null;
            Currency currency = null;
            List<NotebookLabel> notebookLabels = new List<NotebookLabel>();
            if (context.Items.TryGetValue("Notebook", out object notebookObj))
                notebook = (Notebook)notebookObj;
            if (context.Items.TryGetValue("Currency", out object currencyObj))
                currency = (Currency)currencyObj;
            if (context.Items.TryGetValue("NotebookLabels", out object notebookLabelsObj))
                notebookLabels = (List<NotebookLabel>)notebookLabelsObj;

            if (notebook is null || currency is null)
                return null;

            var transactions = JsonSerializer.Deserialize<List<Transaction>>(source.StateData);

            if (!transactions.Any())
                return null;
            
            var transaction = transactions.FirstOrDefault();
            
            var dto = new TransactionDto
            {
                NotebookId = notebook.Id,
                Name = transaction.Name,
                Description = transaction.Description,
                Amount = transaction.Amount,
                TransactionType = transaction.TransactionType,
                IsMatched = transaction.IsMatched,
                IsProceed = false,
                TransactionDate = transaction.TransactionDate,
                Notebook = new NotebookDto
                {
                    Id = notebook.Id,
                    IsDefault = notebook.IsDefault,
                    Name = notebook.Name
                },
                Currency = new CurrencyDto
                {
                    Id = currency.Id,
                    Code = currency.Code,
                    IsSymbolRight = currency.IsSymbolRight,
                    Name = currency.Name,
                    Symbol = currency.Symbol
                },
                Labels = new List<NotebookLabelDto>()
            };

            foreach (var labels in transaction.TransactionLabels)
            {
                var label = notebookLabels.FirstOrDefault(nl => nl.Id == labels.NotebookLabelId);

                if (label is null)
                    continue;

                dto.Labels.Add(new NotebookLabelDto
                {
                    Id = label.Id,
                    Name = label.Name,
                    ColorCode = label.ColorCode
                });
            }

            return dto;
        }
    }
}