using FluentValidation;

namespace ButceYonet.Application.Application.Features.Transactions.UpdateTransaction;

public class UpdateTransactionCommandValidator : AbstractValidator<UpdateTransactionCommand>
{
    public UpdateTransactionCommandValidator()
    {
        RuleFor(p => p.NotebookId).GreaterThan(0);
        RuleFor(p => p.TransactionId).GreaterThan(0);
        RuleFor(p => p.Name)
            .NotEmpty()
            .MaximumLength(100);
        RuleFor(p => p.Description)
            .MaximumLength(1000);
        RuleFor(p => p.CurrencyId)
            .GreaterThan(0);
        RuleFor(p => p.TransactionType).IsInEnum();
    }
}