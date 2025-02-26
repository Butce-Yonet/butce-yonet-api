using DotBoil.Entities;
using MediatR;

namespace ButceYonet.Application.Application.Features.NotebookLabels.CreateNotebookLabel;

public class CreateNotebookLabelCommand : IRequest<BaseResponse>
{
    public int NotebookId { get; set; }
    public string Name { get; set; }
    public string ColorCode { get; set; }
}