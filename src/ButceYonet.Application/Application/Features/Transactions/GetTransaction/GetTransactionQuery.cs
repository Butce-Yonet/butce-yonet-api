using DotBoil.Entities;
using MediatR;

namespace ButceYonet.Application.Application.Features.Transactions.GetTransaction;

public class GetTransactionQuery : IRequest<BaseResponse>
{
    public int TransactionId { get; set; }

    public GetTransactionQuery()
    {
    }

    public GetTransactionQuery(int transactionId)
    {
        TransactionId = transactionId;
    }
}
