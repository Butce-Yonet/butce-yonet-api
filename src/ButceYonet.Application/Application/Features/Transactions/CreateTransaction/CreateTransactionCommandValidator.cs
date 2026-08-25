using ButceYonet.Application.Domain.Enums;
using FluentValidation;

namespace ButceYonet.Application.Application.Features.Transactions.CreateTransaction;

public class CreateTransactionCommandValidator : AbstractValidator<CreateTransactionCommand>
{
    public CreateTransactionCommandValidator()
    {
        RuleFor(p => p.Transactions)
            .NotNull()
            .NotEmpty();

        RuleForEach(p => p.Transactions)
            .ChildRules(item =>
            {
                item.RuleFor(p => p.TransactionType)
                    .NotEqual(TransactionTypes.Saving)
                    .WithMessage("Birikim tipinde işlem yalnızca hedefe katkı ekleyerek oluşturulabilir.");
            });
    }
}