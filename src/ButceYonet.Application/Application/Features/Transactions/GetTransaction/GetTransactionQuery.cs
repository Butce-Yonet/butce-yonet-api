using DotBoil.Entities;
using MediatR;

namespace ButceYonet.Application.Application.Features.Transactions.GetTransaction;

public class GetTransactionQuery : IRequest<BaseResponse>
{
    public int NotebookId { get; set; }
    public int TransactionId { get; set; }

    public GetTransactionQuery()
    {
    }
    
    public GetTransactionQuery(int notebookId, int transactionId)
    {
        NotebookId = notebookId;
        TransactionId = transactionId;
    }
}