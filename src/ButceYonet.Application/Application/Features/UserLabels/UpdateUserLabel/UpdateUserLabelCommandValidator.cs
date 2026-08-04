using FluentValidation;

namespace ButceYonet.Application.Application.Features.UserLabels.UpdateUserLabel;

public class UpdateUserLabelCommandValidator : AbstractValidator<UpdateUserLabelCommand>
{
    public UpdateUserLabelCommandValidator()
    {
        RuleFor(p => p.Id).GreaterThan(0);

        RuleFor(p => p.Name)
            .NotEmpty()
            .MaximumLength(128);

        RuleFor(p => p.ColorCode)
            .NotEmpty()
            .MaximumLength(16);
    }
}