using FluentValidation;

namespace ButceYonet.Application.Application.Features.Subscriptions.DeleteSubscription;

public class DeleteSubscriptionCommandValidator : AbstractValidator<DeleteSubscriptionCommand>
{
    public DeleteSubscriptionCommandValidator()
    {
        RuleFor(p => p.SubscriptionId)
            .GreaterThan(0);
    }
}
