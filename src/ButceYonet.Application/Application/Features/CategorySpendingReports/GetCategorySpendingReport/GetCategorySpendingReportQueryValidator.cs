using FluentValidation;

namespace ButceYonet.Application.Application.Features.CategorySpendingReports.GetCategorySpendingReport;

public class GetCategorySpendingReportQueryValidator : AbstractValidator<GetCategorySpendingReportQuery>
{
    public GetCategorySpendingReportQueryValidator()
    {
        RuleFor(p => p.StartDate)
            .LessThanOrEqualTo(p => p.EndDate)
            .WithMessage("Başlangıç tarihi bitiş tarihinden sonra olamaz.");
    }
}
