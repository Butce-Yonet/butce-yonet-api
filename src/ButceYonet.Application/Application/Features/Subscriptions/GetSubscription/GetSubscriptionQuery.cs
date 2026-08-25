using DotBoil.Entities;
using MediatR;

namespace ButceYonet.Application.Application.Features.Subscriptions.GetSubscription;

public class GetSubscriptionQuery : IRequest<BaseResponse>
{
    public int SubscriptionId { get; set; }

    public GetSubscriptionQuery()
    {
    }

    public GetSubscriptionQuery(int subscriptionId)
    {
        SubscriptionId = subscriptionId;
    }
}
