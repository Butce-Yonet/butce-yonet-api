using DotBoil.Entities;
using MediatR;

namespace ButceYonet.Application.Application.Features.NotebookLabels.GetNotebookLabels;

public class GetNotebookLabelsQuery : IRequest<BaseResponse>
{
    public int NotebookId { get; set; }

    public GetNotebookLabelsQuery()
    {
    }

    public GetNotebookLabelsQuery(int notebookId)
    {
        NotebookId = notebookId;
    }
}