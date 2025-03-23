using ButceYonet.Application.Domain.Enums;
using DotBoil.Entities;
using MediatR;

namespace ButceYonet.Application.Application.Features.RecurringTransactions.CreateRecurringTransaction;

public class CreateRecurringTransactionCommand : IRequest<BaseResponse>
{
    public int NotebookId { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public RecurringTransactionIntervals Frequency { get; set; }
    public int? Interval { get; set; }

    public CreateRecurringTransactionItem Transaction { get; set; }

    public class CreateRecurringTransactionItem
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public decimal Amount { get; set; }
        public int CurrencyId { get; set; }
        public TransactionTypes TransactionType { get; set; }
        public DateTime TransactionDate { get; set; }
        public List<int> Labels { get; set; }

        public CreateRecurringTransactionItem()
        {
            Labels = new List<int>();
        }
    }
}