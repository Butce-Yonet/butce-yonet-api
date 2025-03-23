using DotBoil.Entities;
using MediatR;

namespace ButceYonet.Application.Application.Features.RecurringTransactions.DeleteRecurringTransaction;

public class DeleteRecurringTransactionCommand : IRequest<BaseResponse>
{
    public int NotebookId { get; set; }
    public int RecurringTransactionId { get; set; }
}