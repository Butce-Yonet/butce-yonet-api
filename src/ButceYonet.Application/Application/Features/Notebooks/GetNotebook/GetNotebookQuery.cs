using DotBoil.Entities;
using MediatR;

namespace ButceYonet.Application.Application.Features.Notebooks.GetNotebook;

public class GetNotebookQuery : IRequest<BaseResponse>
{
    public int Id { get; set; }

    public GetNotebookQuery(int id)
    {
        Id = id;
    }
}