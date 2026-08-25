using FluentValidation;

namespace ButceYonet.Application.Application.Features.Subscriptions.GetSubscription;

public class GetSubscriptionQueryValidator : AbstractValidator<GetSubscriptionQuery>
{
    public GetSubscriptionQueryValidator()
    {
        RuleFor(p => p.SubscriptionId)
            .GreaterThan(0);
    }
}
