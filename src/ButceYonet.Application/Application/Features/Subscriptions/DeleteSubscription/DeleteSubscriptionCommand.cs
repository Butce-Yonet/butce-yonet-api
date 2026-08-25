using DotBoil.Entities;
using MediatR;

namespace ButceYonet.Application.Application.Features.Subscriptions.DeleteSubscription;

public class DeleteSubscriptionCommand : IRequest<BaseResponse>
{
    public int SubscriptionId { get; set; }

    public DeleteSubscriptionCommand()
    {
    }

    public DeleteSubscriptionCommand(int subscriptionId)
    {
        SubscriptionId = subscriptionId;
    }
}
