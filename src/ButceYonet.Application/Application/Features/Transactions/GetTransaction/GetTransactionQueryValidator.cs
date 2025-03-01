using FluentValidation;

namespace ButceYonet.Application.Application.Features.Transactions.GetTransaction;

public class GetTransactionQueryValidator : AbstractValidator<GetTransactionQuery>
{
    public GetTransactionQueryValidator()
    {
        RuleFor(p => p.NotebookId).GreaterThan(0);
        RuleFor(p => p.TransactionId).GreaterThan(0);
    }
}