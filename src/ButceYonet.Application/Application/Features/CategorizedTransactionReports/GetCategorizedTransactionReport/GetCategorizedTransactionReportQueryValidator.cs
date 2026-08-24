using FluentValidation;

namespace ButceYonet.Application.Application.Features.CategorizedTransactionReports.GetCategorizedTransactionReport;

public class GetCategorizedTransactionReportQueryValidator : AbstractValidator<GetCategorizedTransactionReportQuery>
{
    public GetCategorizedTransactionReportQueryValidator()
    {
        RuleFor(p => p.TransactionTypes)
            .IsInEnum();
    }
}
