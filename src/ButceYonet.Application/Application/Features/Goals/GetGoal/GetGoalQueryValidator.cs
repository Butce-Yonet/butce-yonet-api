using FluentValidation;

namespace ButceYonet.Application.Application.Features.Goals.GetGoal;

public class GetGoalQueryValidator : AbstractValidator<GetGoalQuery>
{
    public GetGoalQueryValidator()
    {
        RuleFor(p => p.GoalId)
            .GreaterThan(0);
    }
}
