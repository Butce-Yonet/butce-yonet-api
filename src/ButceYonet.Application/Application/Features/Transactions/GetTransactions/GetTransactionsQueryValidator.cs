using FluentValidation;

namespace ButceYonet.Application.Application.Features.Transactions.GetTransactions;

public class GetTransactionsQueryValidator : AbstractValidator<GetTransactionsQuery>
{
    public GetTransactionsQueryValidator()
    {
        RuleFor(p => p.NotebookId).GreaterThan(0);
    }
}