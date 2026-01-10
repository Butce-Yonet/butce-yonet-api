using FluentValidation;

namespace ButceYonet.Application.Application.Features.RecurringTransactions.UpdateRecurringTransaction;

public class UpdateRecurringTransactionCommandValidator : AbstractValidator<UpdateRecurringTransactionCommand>
{
    public UpdateRecurringTransactionCommandValidator()
    {
        RuleFor(p => p.Id).GreaterThan(0);
        RuleFor(p => p.NotebookId).GreaterThan(0);
        RuleFor(p => p.Name).NotEmpty().MaximumLength(128);
        RuleFor(p => p.StartDate).NotNull();
        RuleFor(p => p.Frequency).IsInEnum();
    }
}