using FluentValidation;

namespace ButceYonet.Application.Application.Features.Transactions.GetCalendar;

public class GetCalendarQueryValidator : AbstractValidator<GetCalendarQuery>
{
    public GetCalendarQueryValidator()
    {
        RuleFor(p => p.Year).InclusiveBetween(2000, 2100);
        RuleFor(p => p.Month).InclusiveBetween(1, 12);
        RuleFor(p => p.PreviewCount).InclusiveBetween(1, 50);
    }
}
