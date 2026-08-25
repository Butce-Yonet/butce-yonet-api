using FluentValidation;

namespace ButceYonet.Application.Application.Features.Goals.UpdateGoal;

public class UpdateGoalCommandValidator : AbstractValidator<UpdateGoalCommand>
{
    public UpdateGoalCommandValidator()
    {
        RuleFor(p => p.Id)
            .GreaterThan(0);

        RuleFor(p => p.Name)
            .NotEmpty()
            .MaximumLength(128);

        RuleFor(p => p.TargetAmount)
            .GreaterThan(0);

        RuleFor(p => p.CurrencyId)
            .GreaterThan(0);
    }
}
