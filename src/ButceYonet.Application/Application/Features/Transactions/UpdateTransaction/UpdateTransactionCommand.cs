using ButceYonet.Application.Domain.Enums;
using DotBoil.Entities;
using MediatR;

namespace ButceYonet.Application.Application.Features.Transactions.UpdateTransaction;

public class UpdateTransactionCommand : IRequest<BaseResponse>
{
    public int NotebookId { get; set; }
    public int TransactionId { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public decimal Amount { get; set; }
    public int CurrencyId { get; set; }
    public TransactionTypes TransactionType { get; set; }
    public DateTime TransactionDate { get; set; }
}