using FluentValidation;

namespace ButceYonet.Application.Application.Features.NotebookUsers.GetNotebookUser;

public class GetNotebookUserQueryValidator : AbstractValidator<GetNotebookUserQuery>
{
    public GetNotebookUserQueryValidator()
    {
        RuleFor(p => p.Id).GreaterThan(0);
        RuleFor(p => p.NotebookId).GreaterThan(0);
    }
}