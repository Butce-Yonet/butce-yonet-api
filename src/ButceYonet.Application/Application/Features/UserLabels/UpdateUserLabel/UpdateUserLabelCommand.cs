using DotBoil.Entities;
using MediatR;

namespace ButceYonet.Application.Application.Features.UserLabels.UpdateUserLabel;

public class UpdateUserLabelCommand : IRequest<BaseResponse>
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string ColorCode { get; set; }
}