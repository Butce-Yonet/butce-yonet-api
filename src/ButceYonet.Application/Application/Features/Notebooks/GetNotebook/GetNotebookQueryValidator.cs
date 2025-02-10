using FluentValidation;

namespace ButceYonet.Application.Application.Features.Notebooks.GetNotebook;

public class GetNotebookQueryValidator : AbstractValidator<GetNotebookQuery>
{
    public GetNotebookQueryValidator()
    {
        RuleFor(p => p.Id).GreaterThan(0);
    }
}