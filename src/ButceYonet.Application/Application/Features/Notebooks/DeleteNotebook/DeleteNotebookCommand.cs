using DotBoil.Entities;
using MediatR;

namespace ButceYonet.Application.Application.Features.Notebooks.DeleteNotebook;

public class DeleteNotebookCommand : IRequest<BaseResponse>
{
    public int Id { get; set; }
}