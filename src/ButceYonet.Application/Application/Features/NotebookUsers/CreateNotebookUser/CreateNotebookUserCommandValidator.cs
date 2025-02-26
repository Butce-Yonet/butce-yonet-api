using FluentValidation;

namespace ButceYonet.Application.Application.Features.NotebookUsers.CreateNotebookUser;

public class CreateNotebookUserCommandValidator : AbstractValidator<CreateNotebookUserCommand>
{
    public CreateNotebookUserCommandValidator()
    {
        RuleFor(p => p.NotebookId).GreaterThan(0);
        RuleFor(p => p.Email).NotEmpty().MaximumLength(100);
    }
}