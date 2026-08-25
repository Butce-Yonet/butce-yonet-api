using ButceYonet.Application.Domain.Enums;
using DotBoil.Entities;
using MediatR;

namespace ButceYonet.Application.Application.Features.Subscriptions.UpdateSubscription;

public class UpdateSubscriptionCommand : IRequest<BaseResponse>
{
    public int Id { get; set; }
    public string Name { get; set; }
    public decimal? Amount { get; set; }
    public int? CurrencyId { get; set; }
    public DateTime StartDate { get; set; }
    public RecurringTransactionIntervals Frequency { get; set; }
    public int? Interval { get; set; }
    public List<int> Labels { get; set; }

    public UpdateSubscriptionCommand()
    {
        Labels = new List<int>();
    }
}
