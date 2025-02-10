using DotBoil.Entities;
using MediatR;

namespace ButceYonet.Application.Application.Features.Notebooks.UpdateNotebook;

public class UpdateNotebookCommand : IRequest<BaseResponse>
{
    public int Id { get; set; }
    public string Name { get; set; }
}