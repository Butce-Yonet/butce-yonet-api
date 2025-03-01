using DotBoil.Entities;
using MediatR;

namespace ButceYonet.Application.Application.Features.Transactions.DeleteTransaction;

public class DeleteTransactionCommand : IRequest<BaseResponse>
{
    public int NotebookId { get; set; }
    public int TransactionId { get; set; }

    public DeleteTransactionCommand()
    {
    }

    public DeleteTransactionCommand(int notebookId, int transactionId)
    {
        NotebookId = notebookId;
        TransactionId = transactionId;
    }
}