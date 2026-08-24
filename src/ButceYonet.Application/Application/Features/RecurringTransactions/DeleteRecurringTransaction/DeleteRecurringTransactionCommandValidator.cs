using FluentValidation;

namespace ButceYonet.Application.Application.Features.RecurringTransactions.DeleteRecurringTransaction;

public class DeleteRecurringTransactionCommandValidator : AbstractValidator<DeleteRecurringTransactionCommand>
{
    public DeleteRecurringTransactionCommandValidator()
    {
        RuleFor(p => p.RecurringTransactionId)
            .GreaterThan(0);
    }
}
