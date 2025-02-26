using DotBoil.Entities;
using MediatR;

namespace ButceYonet.Application.Application.Features.NotebookLabels.UpdateNotebookLabel;

public class UpdateNotebookLabelCommand : IRequest<BaseResponse>
{
    public int NotebookId { get; set; }
    public int NotebookLabelId { get; set; }
    public string Name { get; set; }
    public string ColorCode { get; set; }
}