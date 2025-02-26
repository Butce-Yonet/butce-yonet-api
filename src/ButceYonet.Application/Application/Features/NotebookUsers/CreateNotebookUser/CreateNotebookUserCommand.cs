using DotBoil.Entities;
using MediatR;

namespace ButceYonet.Application.Application.Features.NotebookUsers.CreateNotebookUser;

public class CreateNotebookUserCommand : IRequest<BaseResponse>
{
    public int NotebookId { get; set; }
    public string Email { get; set; }
}