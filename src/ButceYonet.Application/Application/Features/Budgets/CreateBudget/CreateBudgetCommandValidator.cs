using ButceYonet.Application.Application.Shared;
using FluentValidation;

namespace ButceYonet.Application.Application.Features.Budgets.CreateBudget;

public class CreateBudgetCommandValidator : AbstractValidator<CreateBudgetCommand>
{
    public CreateBudgetCommandValidator()
    {
        RuleFor(p => p.Amount)
            .GreaterThan(0);

        RuleFor(p => p.CurrencyId)
            .GreaterThan(0);

        RuleFor(p => p.UserLabelId)
            .GreaterThan(0)
            .When(p => p.UserLabelId.HasValue);

        RuleFor(p => p.PeriodStartDay)
            .InclusiveBetween(BudgetPeriodCalculator.MinStartDay, BudgetPeriodCalculator.MaxStartDay);
    }
}
