using FluentValidation;

namespace ButceYonet.Application.Application.Features.NotebookLabels.GetNotebookLabels;

public class GetNotebookLabelsQueryValidator : AbstractValidator<GetNotebookLabelsQuery>
{
    public GetNotebookLabelsQueryValidator()
    {
        RuleFor(p => p.NotebookId).GreaterThan(0);
    }
}