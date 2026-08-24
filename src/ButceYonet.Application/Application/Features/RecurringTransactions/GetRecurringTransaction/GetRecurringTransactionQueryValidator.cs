using FluentValidation;

namespace ButceYonet.Application.Application.Features.RecurringTransactions.GetRecurringTransaction;

public class GetRecurringTransactionQueryValidator : AbstractValidator<GetRecurringTransactionQuery>
{
    public GetRecurringTransactionQueryValidator()
    {
        RuleFor(p => p.RecurringTransactionId)
            .GreaterThan(0);
    }
}
