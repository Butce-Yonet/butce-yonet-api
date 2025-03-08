using FluentValidation;

namespace ButceYonet.Application.Application.Features.Transactions.RemoveTransactionLabel;

public class RemoveTransactionLabelCommandValidator : AbstractValidator<RemoveTransactionLabelCommand>
{
    public RemoveTransactionLabelCommandValidator()
    {
        RuleFor(p => p.NotebookId).GreaterThan(0);
        RuleFor(p => p.TransactionId).GreaterThan(0);
        RuleFor(p => p.LabelId).GreaterThan(0);
    }
}