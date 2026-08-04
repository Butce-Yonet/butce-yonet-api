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

    public class NotebookResolver : IValueResolver<RecurringTransaction, RecurringTransactionDto, NotebookDto>
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

    public class TransactionResolver : IValueResolver<RecurringTransaction, RecurringTransactionDto, TransactionDto>
    {
        public TransactionDto Resolve(RecurringTransaction source, RecurringTransactionDto destination,
            TransactionDto destMember,
            ResolutionContext context)
        {
            Notebook notebook = null;
            Currency currency = null;
            List<UserLabel> userLabels = new List<UserLabel>();

            if (context.Items.TryGetValue("Notebook", out object notebookObj))
                notebook = (Notebook)notebookObj;
            if (context.Items.TryGetValue("Currency", out object currencyObj))
                currency = (Currency)currencyObj;
            if (context.Items.TryGetValue("UserLabels", out object userLabelsObj))
                userLabels = (List<UserLabel>)userLabelsObj;

            if (notebook is null || currency is null)
                return null;

            var transactions = JsonSerializer.Deserialize<List<TransactionV2>>(source.StateData);

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
                    Symbol = currency.Symbol,
                    Rank = currency.Rank
                },
                Labels = new List<UserLabelDto>()
            };

            foreach (var transactionLabel in transaction.TransactionLabelsV2 ?? new List<TransactionLabelV2>())
            {
                var label = userLabels.FirstOrDefault(ul => ul.Id == transactionLabel.UserLabelId);

                if (label is null)
                    continue;

                dto.Labels.Add(new UserLabelDto
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