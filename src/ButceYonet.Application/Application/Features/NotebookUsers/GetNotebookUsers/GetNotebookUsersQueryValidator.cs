using FluentValidation;

namespace ButceYonet.Application.Application.Features.NotebookUsers.GetNotebookUsers;

public class GetNotebookUsersQueryValidator : AbstractValidator<GetNotebookUsersQuery>
{
    public GetNotebookUsersQueryValidator()
    {
        RuleFor(p => p.NotebookId).GreaterThan(0);
    }
}