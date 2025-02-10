using DotBoil.Entities;
using MediatR;

namespace ButceYonet.Application.Application.Features.Notebooks.CreateNotebook;

public class CreateNotebookCommand : IRequest<BaseResponse>
{
    public string Name { get; set; }
}