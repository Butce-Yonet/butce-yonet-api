using DotBoil.Entities;
using MediatR;

namespace ButceYonet.Application.Application.Features.RecurringTransactions.DeleteRecurringTransaction;

public class DeleteRecurringTransactionCommand : IRequest<BaseResponse>
{
    public int NotebookId { get; set; }
    public int RecurringTransactionId { get; set; }

    public DeleteRecurringTransactionCommand()
    {
    }

    public DeleteRecurringTransactionCommand(int notebookId, int recurringTransactionId)
    {
        NotebookId = notebookId;
        RecurringTransactionId = recurringTransactionId;
    }
}