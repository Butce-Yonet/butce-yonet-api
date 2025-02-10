using FluentValidation;

namespace ButceYonet.Application.Application.Features.Notebooks.CreateNotebook;

public class CreateNotebookCommandValidator : AbstractValidator<CreateNotebookCommand>
{
    public CreateNotebookCommandValidator()
    {
        RuleFor(p => p.Name)
            .NotEmpty()
            .MaximumLength(50);
    }
}