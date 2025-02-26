using ButceYonet.Application.Application.Features.NotebookLabels.CreateNotebookLabel;
using ButceYonet.Application.Application.Features.NotebookLabels.DeleteNotebookLabel;
using ButceYonet.Application.Application.Features.NotebookLabels.GetNotebookLabel;
using ButceYonet.Application.Application.Features.NotebookLabels.GetNotebookLabels;
using ButceYonet.Application.Application.Features.NotebookLabels.UpdateNotebookLabel;
using ButceYonet.Application.Application.Shared.Dtos;
using DotBoil.Entities;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace ButceYonet.Api.Controllers;

[Route("api/notebooks/{notebookId}/labels")]
public class NotebookLabelsController : BaseController
{
    public NotebookLabelsController(IMediator mediator) : base(mediator)
    {
    }

    /// <summary>
    /// İstenilen deftere ait tüm isim etiketlerini getirmek amacıyla kullanılır
    /// </summary>
    /// <param name="notebookId">İstenilen defterin id</param>
    /// <returns></returns>
    [HttpGet]
    [ProducesResponseType(typeof(BaseResponse<IEnumerable<NotebookLabelDto>>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(BaseResponse<object>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(BaseResponse<object>), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(BaseResponse<object>), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(BaseResponse<object>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Get(int notebookId)
    {
        var query = new GetNotebookLabelsQuery(notebookId);
        var response = await _mediator.Send(query);
        return Response(response);
    }

    /// <summary>
    /// İstenilen deftere ait spesifik bir isim etiketini getirmek için kullanılır
    /// </summary>
    /// <param name="notebookId"></param>
    /// <param name="notebookLabelId"></param>
    /// <returns></returns>
    [HttpGet("{notebookLabelId}")]
    [ProducesResponseType(typeof(BaseResponse<NotebookLabelDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(BaseResponse<object>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(BaseResponse<object>), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(BaseResponse<object>), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(BaseResponse<object>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Get(int notebookId, int notebookLabelId)
    {
        var query = new GetNotebookLabelQuery(notebookId, notebookLabelId);
        var response = await _mediator.Send(query);
        return Response(response);
    }

    /// <summary>
    /// İstenilen deftere ait isim etiketi oluşturmak için kullanılır
    /// </summary>
    /// <param name="notebookId"></param>
    /// <param name="command"></param>
    /// <returns></returns>
    [HttpPost]
    [ProducesResponseType(typeof(BaseResponse<object>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(BaseResponse<object>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(BaseResponse<object>), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(BaseResponse<object>), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(BaseResponse<object>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Create(int notebookId, [FromBody] CreateNotebookLabelCommand command)
    {
        command.NotebookId = notebookId;
        var response = await _mediator.Send(command);
        return Response(response);
    }

    /// <summary>
    /// İstenilen deftere ait isim etiketini güncellemek için kullanılır
    /// </summary>
    /// <param name="notebookId"></param>
    /// <param name="notebookLabelId"></param>
    /// <param name="command"></param>
    /// <returns></returns>
    [HttpPut("{notebookLabelId}")]
    [ProducesResponseType(typeof(BaseResponse<object>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(BaseResponse<object>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(BaseResponse<object>), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(BaseResponse<object>), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(BaseResponse<object>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Update(
        int notebookId,
        int notebookLabelId,
        [FromBody] UpdateNotebookLabelCommand command)
    {
        command.NotebookId = notebookId;
        command.NotebookLabelId = notebookLabelId;
        var response = await _mediator.Send(command);
        return Response(response);
    }

    /// <summary>
    /// İstenilen deftere ait isim etiketini silmek için kullanılır
    /// </summary>
    /// <param name="notebookId"></param>
    /// <param name="notebookLabelId"></param>
    /// <returns></returns>
    [HttpDelete("{notebookLabelId}")]
    [ProducesResponseType(typeof(BaseResponse<object>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(BaseResponse<object>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(BaseResponse<object>), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(BaseResponse<object>), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(BaseResponse<object>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(int notebookId, int notebookLabelId)
    {
        var request = new DeleteNotebookLabelCommand(notebookId, notebookLabelId);
        var response = await _mediator.Send(request);
        return Response(response);
    }
}