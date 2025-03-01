using ButceYonet.Application.Application.Features.NotebookUsers.CreateNotebookUser;
using ButceYonet.Application.Application.Features.NotebookUsers.DeleteNotebookUser;
using ButceYonet.Application.Application.Features.NotebookUsers.GetNotebookUser;
using ButceYonet.Application.Application.Features.NotebookUsers.GetNotebookUsers;
using ButceYonet.Application.Application.Shared.Dtos;
using DotBoil.Entities;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace ButceYonet.Api.Controllers;

[Route("api/notebooks/{notebookId}/labels")]
public class NotebookUsersController : BaseController
{
    public NotebookUsersController(IMediator mediator) : base(mediator)
    {
    }

    /// <summary>
    /// İstenilen deftere ait tüm kullanıcıları getirmek amacıyla kullanılır
    /// </summary>
    /// <param name="notebookId">İstenilen defterin id</param>
    /// <returns></returns>
    [HttpGet]
    [ProducesResponseType(typeof(BaseResponse<IEnumerable<NotebookUserDto>>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(BaseResponse<object>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(BaseResponse<object>), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(BaseResponse<object>), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(BaseResponse<object>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Get(int notebookId)
    {
        var request = new GetNotebookUsersQuery(notebookId);
        var response = await _mediator.Send(request);
        return Response(response);
    }

    /// <summary>
    /// İstenilen deftere ait spesifik bir kullanıcıyı getirmek amacıyla kullanılır
    /// </summary>
    /// <param name="notebookId"></param>
    /// <param name="notebookUserId"></param>
    /// <returns></returns>
    [HttpGet("{notebookUserId}")]
    [ProducesResponseType(typeof(BaseResponse<NotebookUserDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(BaseResponse<object>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(BaseResponse<object>), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(BaseResponse<object>), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(BaseResponse<object>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Get(int notebookId, int notebookUserId)
    {
        var request = new GetNotebookUserQuery(notebookUserId, notebookId);
        var response = await _mediator.Send(request);
        return Response(response);
    }

    /// <summary>
    /// İstenilen deftere ait kullanıcı eklemek için kullanılır
    /// </summary>
    /// <param name="notebookId"></param>
    /// <param name="request"></param>
    /// <returns></returns>
    [HttpPost]
    [ProducesResponseType(typeof(BaseResponse<object>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(BaseResponse<object>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(BaseResponse<object>), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(BaseResponse<object>), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(BaseResponse<object>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Create(int notebookId, [FromBody] CreateNotebookUserCommand request)
    {
        request.NotebookId = notebookId;
        var response = await _mediator.Send(request);
        return Response(response);
    }

    /// <summary>
    /// İstenilen deftere ait kullanıcıyı silmek için kullanılır
    /// </summary>
    /// <param name="notebookId"></param>
    /// <param name="userId"></param>
    /// <returns></returns>
    [HttpDelete("{userId}")]
    [ProducesResponseType(typeof(BaseResponse<object>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(BaseResponse<object>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(BaseResponse<object>), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(BaseResponse<object>), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(BaseResponse<object>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(int notebookId, int userId)
    {
        var request = new DeleteNotebookUserCommand(notebookId, userId);
        var response = await _mediator.Send(request);
        return Response(response);
    }
}