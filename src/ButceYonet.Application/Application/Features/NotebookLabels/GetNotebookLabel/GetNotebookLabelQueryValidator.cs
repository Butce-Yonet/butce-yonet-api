using FluentValidation;

namespace ButceYonet.Application.Application.Features.NotebookLabels.GetNotebookLabel;

public class GetNotebookLabelQueryValidator : AbstractValidator<GetNotebookLabelQuery>
{
    public GetNotebookLabelQueryValidator()
    {
        RuleFor(p => p.NotebookId).GreaterThan(0);
        RuleFor(p => p.NotebookLabelId).GreaterThan(0);
    }
}