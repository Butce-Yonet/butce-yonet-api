using DotBoil.Entities;
using MediatR;

namespace ButceYonet.Application.Application.Features.RecurringTransactions.GetRecurringTransaction;

public class GetRecurringTransactionQuery : PaginationFilter, IRequest<BaseResponse>
{
    public int RecurringTransactionId { get; set; }

    public GetRecurringTransactionQuery()
    {
    }

    public GetRecurringTransactionQuery(int recurringTransactionId)
    {
        RecurringTransactionId = recurringTransactionId;
    }
}
