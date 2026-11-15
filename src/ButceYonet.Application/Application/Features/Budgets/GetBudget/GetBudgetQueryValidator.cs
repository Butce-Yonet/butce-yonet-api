using FluentValidation;

namespace ButceYonet.Application.Application.Features.Budgets.GetBudget;

public class GetBudgetQueryValidator : AbstractValidator<GetBudgetQuery>
{
    public GetBudgetQueryValidator()
    {
        RuleFor(p => p.BudgetId)
            .GreaterThan(0);

        RuleFor(p => p.HistoryCount)
            .InclusiveBetween(1, 12);
    }
}
