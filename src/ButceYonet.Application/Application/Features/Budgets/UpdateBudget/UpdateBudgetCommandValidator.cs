using ButceYonet.Application.Application.Shared;
using FluentValidation;

namespace ButceYonet.Application.Application.Features.Budgets.UpdateBudget;

public class UpdateBudgetCommandValidator : AbstractValidator<UpdateBudgetCommand>
{
    public UpdateBudgetCommandValidator()
    {
        RuleFor(p => p.Id)
            .GreaterThan(0);

        RuleFor(p => p.Amount)
            .GreaterThan(0);

        RuleFor(p => p.PeriodStartDay)
            .InclusiveBetween(BudgetPeriodCalculator.MinStartDay, BudgetPeriodCalculator.MaxStartDay);
    }
}
