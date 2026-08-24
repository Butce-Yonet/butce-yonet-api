using FluentValidation;

namespace ButceYonet.Application.Application.Features.Transactions.GetTransaction;

public class GetTransactionQueryValidator : AbstractValidator<GetTransactionQuery>
{
    public GetTransactionQueryValidator()
    {
        RuleFor(p => p.TransactionId).GreaterThan(0);
    }
}
