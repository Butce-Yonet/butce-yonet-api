using DotBoil.Entities;
using MediatR;

namespace ButceYonet.Application.Application.Features.RecurringTransactions.GetRecurringTransaction;

public class GetRecurringTransactionQuery : PaginationFilter, IRequest<BaseResponse>
{
    public int NotebookId { get; set; }
    public int RecurringTransactionId { get; set; }

    public GetRecurringTransactionQuery()
    {
    }

    public GetRecurringTransactionQuery(int notebookId, int recurringTransactionId)
    {
        NotebookId = notebookId;
        RecurringTransactionId = recurringTransactionId;
    }
}