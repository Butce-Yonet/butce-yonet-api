using FluentValidation;

namespace ButceYonet.Application.Application.Features.Subscriptions.GetSubscriptions;

public class GetSubscriptionsQueryValidator : AbstractValidator<GetSubscriptionsQuery>
{
    public GetSubscriptionsQueryValidator()
    {
        RuleFor(p => p.Status)
            .IsInEnum()
            .When(p => p.Status.HasValue);
    }
}
