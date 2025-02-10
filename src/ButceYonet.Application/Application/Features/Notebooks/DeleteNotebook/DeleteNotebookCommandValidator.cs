using FluentValidation;

namespace ButceYonet.Application.Application.Features.Notebooks.DeleteNotebook;

public class DeleteNotebookCommandValidator : AbstractValidator<DeleteNotebookCommand>
{
    public DeleteNotebookCommandValidator()
    {
        RuleFor(p => p.Id).GreaterThan(0);
    }
}