using FluentValidation;

namespace ButceYonet.Application.Application.Features.Transactions.CreateTransaction;

public class CreateTransactionCommandValidator : AbstractValidator<CreateTransactionCommand>
{
    public CreateTransactionCommandValidator()
    {
        RuleFor(p => p.NotebookId)
            .GreaterThan(0);
        
        RuleFor(p => p.Transactions)
            .NotNull()
            .NotEmpty();
    }
}