using FluentValidation;

namespace ButceYonet.Application.Application.Features.Goals.DeleteGoal;

public class DeleteGoalCommandValidator : AbstractValidator<DeleteGoalCommand>
{
    public DeleteGoalCommandValidator()
    {
        RuleFor(p => p.GoalId)
            .GreaterThan(0);
    }
}
