using DotBoil.Entities;
using MediatR;

namespace ButceYonet.Application.Application.Features.NotebookLabels.DeleteNotebookLabel;

public class DeleteNotebookLabelCommand : IRequest<BaseResponse>
{
    public int NotebookId { get; set; }
    public int NotebookLabelId { get; set; }

    public DeleteNotebookLabelCommand()
    {
    }

    public DeleteNotebookLabelCommand(int notebookId, int notebookLabelId)
    {
        NotebookId = notebookId;
        notebookLabelId = notebookLabelId;
    }
}