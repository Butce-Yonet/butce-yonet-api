using ButceYonet.Application.Domain.Enums;
using DotBoil.Entities;
using MediatR;

namespace ButceYonet.Application.Application.Features.Subscriptions.CreateSubscription;

public class CreateSubscriptionCommand : IRequest<BaseResponse>
{
    public string Name { get; set; }
    public decimal? Amount { get; set; }
    public int? CurrencyId { get; set; }
    public DateTime StartDate { get; set; }
    public RecurringTransactionIntervals Frequency { get; set; }
    public int? Interval { get; set; }
    public List<int> Labels { get; set; }

    public CreateSubscriptionCommand()
    {
        Labels = new List<int>();
    }
}
