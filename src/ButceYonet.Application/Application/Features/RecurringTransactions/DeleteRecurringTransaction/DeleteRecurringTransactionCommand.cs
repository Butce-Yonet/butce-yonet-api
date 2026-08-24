using DotBoil.Entities;
using MediatR;

namespace ButceYonet.Application.Application.Features.RecurringTransactions.DeleteRecurringTransaction;

public class DeleteRecurringTransactionCommand : IRequest<BaseResponse>
{
    public int RecurringTransactionId { get; set; }

    public DeleteRecurringTransactionCommand()
    {
    }

    public DeleteRecurringTransactionCommand(int recurringTransactionId)
    {
        RecurringTransactionId = recurringTransactionId;
    }
}
