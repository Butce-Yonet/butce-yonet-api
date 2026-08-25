using FluentValidation;

namespace ButceYonet.Application.Application.Features.Goals.CreateGoal;

public class CreateGoalCommandValidator : AbstractValidator<CreateGoalCommand>
{
    public CreateGoalCommandValidator()
    {
        RuleFor(p => p.Name)
            .NotEmpty()
            .MaximumLength(128);

        RuleFor(p => p.TargetAmount)
            .GreaterThan(0);

        RuleFor(p => p.CurrencyId)
            .GreaterThan(0);
    }
}
