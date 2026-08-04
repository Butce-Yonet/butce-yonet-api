using FluentValidation;

namespace ButceYonet.Application.Application.Features.UserLabels.DeleteUserLabel;

public class DeleteUserLabelCommandValidator : AbstractValidator<DeleteUserLabelCommand>
{
    public DeleteUserLabelCommandValidator()
    {
        RuleFor(p => p.Id).GreaterThan(0);
    }
}