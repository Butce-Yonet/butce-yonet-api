using ButceYonet.Application.Domain.Enums;
using DotBoil.Entities;
using MediatR;

namespace ButceYonet.Application.Application.Features.Transactions.CreateTransaction;

public class CreateTransactionCommand : IRequest<BaseResponse>
{
    public int NotebookId { get; set; }
    public List<CreateTransactionItem> Transactions { get; set; }
    
    public class CreateTransactionItem
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public decimal Amount { get; set; }
        public int CurrencyId { get; set; }
        public TransactionTypes TransactionType { get; set; }
        public DateTime TransactionDate { get; set; }
        public List<int> Labels { get; set; }

        public CreateTransactionItem()
        {
            Labels = new List<int>();
        }
    }

    public CreateTransactionCommand()
    {
        Transactions = new List<CreateTransactionItem>();
    }
}