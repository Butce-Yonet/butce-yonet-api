using DotBoil.Entities;
using MediatR;

namespace ButceYonet.Application.Application.Features.NotebookLabels.GetNotebookLabel;

public class GetNotebookLabelQuery : IRequest<BaseResponse>
{
    public int NotebookId { get; set; }
    public int NotebookLabelId { get; set; }

    public GetNotebookLabelQuery()
    {
    }

    public GetNotebookLabelQuery(int notebookId, int notebookLabelId)
    {
        NotebookId = notebookId;
        NotebookLabelId = notebookLabelId;
    }
}