using DotBoil.Entities;
using MediatR;

namespace ButceYonet.Application.Application.Features.UserLabels.CreateUserLabel;

public class CreateUserLabelCommand : IRequest<BaseResponse>
{
    public string Name { get; set; }
    public string ColorCode { get; set; }
}