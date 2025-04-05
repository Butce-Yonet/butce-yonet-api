using ButceYonet.Application.Domain.Enums;
using DotBoil.Entities;
using MediatR;

namespace ButceYonet.Application.Application.Features.RecurringTransactions.UpdateRecurringTransaction;

public class UpdateRecurringTransactionCommand : IRequest<BaseResponse>
{
    public int Id { get; set; }
    public int NotebookId { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public RecurringTransactionIntervals Frequency { get; set; }
    public int? Interval { get; set; }
    public DateTime? NextOccurence { get; set; }
}