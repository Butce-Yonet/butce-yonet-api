using FluentValidation;

namespace ButceYonet.Application.Application.Features.UserLabels.CreateUserLabel;

public class CreateUserLabelCommandValidator : AbstractValidator<CreateUserLabelCommand>
{
    public CreateUserLabelCommandValidator()
    {
        RuleFor(p => p.Name)
            .NotEmpty()
            .MaximumLength(128);

        RuleFor(p => p.ColorCode)
            .NotEmpty()
            .MaximumLength(16);
    }
}