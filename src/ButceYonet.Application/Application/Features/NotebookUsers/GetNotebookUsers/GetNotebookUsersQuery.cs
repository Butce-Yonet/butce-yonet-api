using DotBoil.Entities;
using MediatR;

namespace ButceYonet.Application.Application.Features.NotebookUsers.GetNotebookUsers;

public class GetNotebookUsersQuery : IRequest<BaseResponse>
{
    public int NotebookId { get; set; }

    public GetNotebookUsersQuery()
    {
    }

    public GetNotebookUsersQuery(int notebookId)
    {
        NotebookId = notebookId;
    }
}