using FluentValidation;

namespace ButceYonet.Application.Application.Features.RecurringTransactions.CreateRecurringTransaction;

public class CreateRecurringTransactionCommandValidator : AbstractValidator<CreateRecurringTransactionCommand>
{
    public CreateRecurringTransactionCommandValidator()
    {
        RuleFor(p => p.NotebookId)
            .GreaterThan(0);

        RuleFor(p => p.Name)
            .NotEmpty()
            .MaximumLength(128);

        RuleFor(p => p.StartDate)
            .NotNull();

        RuleFor(p => p.Frequency)
            .IsInEnum();

        RuleFor(p => p.Transaction)
            .NotNull();
    }
}