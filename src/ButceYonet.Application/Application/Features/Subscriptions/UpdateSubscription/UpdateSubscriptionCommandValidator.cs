using FluentValidation;

namespace ButceYonet.Application.Application.Features.Subscriptions.UpdateSubscription;

public class UpdateSubscriptionCommandValidator : AbstractValidator<UpdateSubscriptionCommand>
{
    public UpdateSubscriptionCommandValidator()
    {
        RuleFor(p => p.Id)
            .GreaterThan(0);

        RuleFor(p => p.Name)
            .NotEmpty()
            .MaximumLength(128);

        RuleFor(p => p.Amount)
            .GreaterThan(0)
            .When(p => p.Amount.HasValue);

        RuleFor(p => p.CurrencyId)
            .NotNull()
            .WithMessage("Tutar belirtildiğinde para birimi de belirtilmelidir.")
            .When(p => p.Amount.HasValue);

        RuleFor(p => p.Frequency)
            .IsInEnum();
    }
}
