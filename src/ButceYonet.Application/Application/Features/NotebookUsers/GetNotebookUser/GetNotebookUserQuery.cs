using DotBoil.Entities;
using MediatR;

namespace ButceYonet.Application.Application.Features.NotebookUsers.GetNotebookUser;

public class GetNotebookUserQuery : IRequest<BaseResponse>
{
    public int Id { get; set; }
    public int NotebookId { get; set; }

    public GetNotebookUserQuery()
    {
    }

    public GetNotebookUserQuery(int id, int notebookId)
    {
        Id = id;
        NotebookId = notebookId;
    }
}