using FluentValidation;

namespace ButceYonet.Application.Application.Features.UserLabels.GetUserLabel;

public class GetUserLabelQueryValidator : AbstractValidator<GetUserLabelQuery>
{
    public GetUserLabelQueryValidator()
    {
        RuleFor(p => p.Id).GreaterThan(0);
    }
}