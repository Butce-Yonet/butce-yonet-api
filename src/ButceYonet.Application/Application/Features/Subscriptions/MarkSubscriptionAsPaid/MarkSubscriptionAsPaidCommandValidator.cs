using FluentValidation;

namespace ButceYonet.Application.Application.Features.Subscriptions.MarkSubscriptionAsPaid;

public class MarkSubscriptionAsPaidCommandValidator : AbstractValidator<MarkSubscriptionAsPaidCommand>
{
    public MarkSubscriptionAsPaidCommandValidator()
    {
        RuleFor(p => p.SubscriptionId)
            .GreaterThan(0);

        RuleFor(p => p.Amount)
            .GreaterThan(0);

        RuleFor(p => p.CurrencyId)
            .GreaterThan(0);
    }
}
