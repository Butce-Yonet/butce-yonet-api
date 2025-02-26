using FluentValidation;

namespace ButceYonet.Application.Application.Features.NotebookLabels.UpdateNotebookLabel;

public class UpdateNotebookLabelCommandValidator : AbstractValidator<UpdateNotebookLabelCommand>
{
    public UpdateNotebookLabelCommandValidator()
    {
        RuleFor(p => p.NotebookId).GreaterThan(0);
        RuleFor(p => p.NotebookLabelId).GreaterThan(0);
        RuleFor(p => p.Name).NotEmpty().MaximumLength(128);
        RuleFor(p => p.ColorCode).NotEmpty().MaximumLength(16);
    }
}