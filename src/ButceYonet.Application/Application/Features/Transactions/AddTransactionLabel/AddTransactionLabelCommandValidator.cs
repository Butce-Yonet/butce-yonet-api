using FluentValidation;

namespace ButceYonet.Application.Application.Features.Transactions.AddTransactionLabel;

public class AddTransactionLabelCommandValidator : AbstractValidator<AddTransactionLabelCommand>
{
    public AddTransactionLabelCommandValidator()
    {
        RuleFor(p => p.NotebookId).GreaterThan(0);
        RuleFor(p => p.TransactionId).GreaterThan(0);
        RuleFor(p => p.LabelId).GreaterThan(0);
    }
}