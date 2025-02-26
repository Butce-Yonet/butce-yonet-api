using FluentValidation;

namespace ButceYonet.Application.Application.Features.NotebookLabels.CreateNotebookLabel;

public class CreateNotebookLabelCommandValidator : AbstractValidator<CreateNotebookLabelCommand>
{
    public CreateNotebookLabelCommandValidator()
    {
        RuleFor(p => p.NotebookId).GreaterThan(0);
        RuleFor(p => p.Name).NotEmpty().MaximumLength(128);
        RuleFor(p => p.ColorCode).NotEmpty().MaximumLength(16);
    }
}