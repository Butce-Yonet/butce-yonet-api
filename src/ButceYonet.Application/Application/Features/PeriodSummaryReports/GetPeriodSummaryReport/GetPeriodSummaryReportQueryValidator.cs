using FluentValidation;

namespace ButceYonet.Application.Application.Features.PeriodSummaryReports.GetPeriodSummaryReport;

public class GetPeriodSummaryReportQueryValidator : AbstractValidator<GetPeriodSummaryReportQuery>
{
    public GetPeriodSummaryReportQueryValidator()
    {
        RuleFor(p => p.NotebookId)
            .GreaterThan(0);

        RuleFor(p => p.StartDate)
            .LessThanOrEqualTo(p => p.EndDate)
            .WithMessage("Başlangıç tarihi bitiş tarihinden sonra olamaz.");
    }
}
