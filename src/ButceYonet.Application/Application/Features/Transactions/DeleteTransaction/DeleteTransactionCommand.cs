using DotBoil.Entities;
using MediatR;

namespace ButceYonet.Application.Application.Features.Transactions.DeleteTransaction;

public class DeleteTransactionCommand : IRequest<BaseResponse>
{
    public int TransactionId { get; set; }

    public DeleteTransactionCommand()
    {
    }

    public DeleteTransactionCommand(int transactionId)
    {
        TransactionId = transactionId;
    }
}
