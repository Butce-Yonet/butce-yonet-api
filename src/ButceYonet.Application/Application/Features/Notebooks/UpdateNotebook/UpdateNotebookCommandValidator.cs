using FluentValidation;

namespace ButceYonet.Application.Application.Features.Notebooks.UpdateNotebook;

public class UpdateNotebookCommandValidator : AbstractValidator<UpdateNotebookCommand>
{
    public UpdateNotebookCommandValidator()
    {
        RuleFor(p => p.Id)
            .GreaterThan(0);
        
        RuleFor(p => p.Name)
            .NotEmpty()
            .MaximumLength(50);
    }
}