using DotBoil.Entities;
using MediatR;

namespace ButceYonet.Application.Application.Features.UserLabels.DeleteUserLabel;

public class DeleteUserLabelCommand : IRequest<BaseResponse>
{
    public int Id { get; set; }
}