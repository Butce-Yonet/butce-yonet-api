using FluentValidation;

namespace ButceYonet.Application.Application.Features.NonCategorizedTransactionReports.GetNonCategorizedTransactionReport;

public class GetNonCategorizedTransactionReportQueryValidator : AbstractValidator<GetNonCategorizedTransactionReportQuery>
{
    public GetNonCategorizedTransactionReportQueryValidator()
    {
        RuleFor(p => p.TransactionTypes)
            .IsInEnum();
    }
}
