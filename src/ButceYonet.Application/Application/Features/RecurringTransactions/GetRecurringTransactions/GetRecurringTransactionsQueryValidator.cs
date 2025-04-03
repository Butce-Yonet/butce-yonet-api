using ButceYonet.Application.Application.Features.RecurringTransactions.GetRecurringTransaction;
using FluentValidation;

namespace ButceYonet.Application.Application.Features.RecurringTransactions.GetRecurringTransactions;

public class GetRecurringTransactionsQueryValidator : AbstractValidator<GetRecurringTransactionQuery>
{
    public GetRecurringTransactionsQueryValidator()
    {
        RuleFor(p => p.NotebookId).GreaterThan(0);
    }
}