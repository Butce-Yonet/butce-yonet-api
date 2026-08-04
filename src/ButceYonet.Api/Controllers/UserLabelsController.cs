using ButceYonet.Application.Application.Features.UserLabels.CreateUserLabel;
using ButceYonet.Application.Application.Features.UserLabels.DeleteUserLabel;
using ButceYonet.Application.Application.Features.UserLabels.GetUserLabel;
using ButceYonet.Application.Application.Features.UserLabels.GetUserLabels;
using ButceYonet.Application.Application.Features.UserLabels.UpdateUserLabel;
using ButceYonet.Application.Application.Shared.Dtos;
using DotBoil.Entities;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace ButceYonet.Api.Controllers;

public class UserLabelsController : BaseController
{
    public UserLabelsController(IMediator mediator) : base(mediator)
    {
    }

    [HttpGet]
    [ProducesResponseType(typeof(BaseResponse<List<UserLabelDto>>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(BaseResponse<object>), StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> GetAll()
    {
        var response = await _mediator.Send(new GetUserLabelsQuery());
        return Response(response);
    }

    [HttpGet("{id}")]
    [ProducesResponseType(typeof(BaseResponse<UserLabelDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(BaseResponse<object>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(BaseResponse<object>), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(BaseResponse<object>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(int id)
    {
        var response = await _mediator.Send(new GetUserLabelQuery(id));
        return Response(response);
    }

    [HttpPost]
    [ProducesResponseType(typeof(BaseResponse<object>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(BaseResponse<object>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(BaseResponse<object>), StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> Create([FromBody] CreateUserLabelCommand command)
    {
        var response = await _mediator.Send(command);
        return Response(response);
    }

    [HttpPut]
    [ProducesResponseType(typeof(BaseResponse<object>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(BaseResponse<object>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(BaseResponse<object>), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(BaseResponse<object>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Update([FromBody] UpdateUserLabelCommand command)
    {
        var response = await _mediator.Send(command);
        return Response(response);
    }

    [HttpDelete("{id}")]
    [ProducesResponseType(typeof(BaseResponse<object>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(BaseResponse<object>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(BaseResponse<object>), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(BaseResponse<object>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(int id)
    {
        var response = await _mediator.Send(new DeleteUserLabelCommand { Id = id });
        return Response(response);
    }
}