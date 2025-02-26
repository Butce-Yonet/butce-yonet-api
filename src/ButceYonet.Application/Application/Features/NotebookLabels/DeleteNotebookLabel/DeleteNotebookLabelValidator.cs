using FluentValidation;

namespace ButceYonet.Application.Application.Features.NotebookLabels.DeleteNotebookLabel;

public class DeleteNotebookLabelValidator : AbstractValidator<DeleteNotebookLabelCommand>
{
    public DeleteNotebookLabelValidator()
    {
        RuleFor(p => p.NotebookId).GreaterThan(0);
        RuleFor(p => p.NotebookLabelId).GreaterThan(0);
    }
}