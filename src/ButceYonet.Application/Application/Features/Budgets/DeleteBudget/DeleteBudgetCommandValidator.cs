using FluentValidation;

namespace ButceYonet.Application.Application.Features.Budgets.DeleteBudget;

public class DeleteBudgetCommandValidator : AbstractValidator<DeleteBudgetCommand>
{
    public DeleteBudgetCommandValidator()
    {
        RuleFor(p => p.BudgetId)
            .GreaterThan(0);
    }
}
