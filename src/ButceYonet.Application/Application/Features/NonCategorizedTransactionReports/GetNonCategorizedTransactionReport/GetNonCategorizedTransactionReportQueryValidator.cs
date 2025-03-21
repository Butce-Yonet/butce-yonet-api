using FluentValidation;

namespace ButceYonet.Application.Application.Features.NonCategorizedTransactionReports.GetNonCategorizedTransactionReport;

public class GetNonCategorizedTransactionReportQueryValidator : AbstractValidator<GetNonCategorizedTransactionReportQuery>
{
    public GetNonCategorizedTransactionReportQueryValidator()
    {
        RuleFor(p => p.NotebookId)
            .GreaterThan(0);

        RuleFor(p => p.TransactionTypes)
            .IsInEnum();
    }
}