using DotBoil.Entities;
using MediatR;

namespace ButceYonet.Application.Application.Features.NotebookUsers.DeleteNotebookUser;

public class DeleteNotebookUserCommand : IRequest<BaseResponse>
{
    public int NotebookId { get; set; }
    public int UserId { get; set; }

    public DeleteNotebookUserCommand()
    {
    }

    public DeleteNotebookUserCommand(int notebookId, int userId)
    {
        NotebookId = notebookId;
        UserId = userId;
    }
}