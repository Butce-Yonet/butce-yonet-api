using FluentValidation;

namespace ButceYonet.Application.Application.Features.NotebookUsers.DeleteNotebookUser;

public class DeleteNotebookUserCommandValidator : AbstractValidator<DeleteNotebookUserCommand>
{
    public DeleteNotebookUserCommandValidator()
    {
        RuleFor(p => p.NotebookId).GreaterThan(0);
        RuleFor(p => p.UserId).GreaterThan(0);
    }
}