using DotBoil.Entities;
using MediatR;

namespace ButceYonet.Application.Application.Features.UserLabels.GetUserLabel;

public class GetUserLabelQuery : IRequest<BaseResponse>
{
    public int Id { get; set; }

    public GetUserLabelQuery(int id)
    {
        Id = id;
    }
}