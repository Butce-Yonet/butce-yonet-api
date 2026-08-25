using FluentValidation;

namespace ButceYonet.Application.Application.Features.Goals.ContributeToGoal;

public class ContributeToGoalCommandValidator : AbstractValidator<ContributeToGoalCommand>
{
    public ContributeToGoalCommandValidator()
    {
        RuleFor(p => p.GoalId)
            .GreaterThan(0);

        RuleFor(p => p.Amount)
            .GreaterThan(0);
    }
}
