using FluentValidation;

namespace ButceYonet.Application.Application.Features.Transactions.DeleteTransaction;

public class DeleteTransactionCommandValidator : AbstractValidator<DeleteTransactionCommand>
{
    public DeleteTransactionCommandValidator()
    {
        RuleFor(p => p.NotebookId).GreaterThan(0);
        RuleFor(p => p.TransactionId).GreaterThan(0);
    }
}