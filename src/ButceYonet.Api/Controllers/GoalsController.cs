using ButceYonet.Application.Application.Features.Goals.ContributeToGoal;
using ButceYonet.Application.Application.Features.Goals.CreateGoal;
using ButceYonet.Application.Application.Features.Goals.DeleteGoal;
using ButceYonet.Application.Application.Features.Goals.GetGoal;
using ButceYonet.Application.Application.Features.Goals.GetGoalMetrics;
using ButceYonet.Application.Application.Features.Goals.GetGoals;
using ButceYonet.Application.Application.Features.Goals.UpdateGoal;
using ButceYonet.Application.Application.Shared.Dtos;
using DotBoil.Entities;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace ButceYonet.Api.Controllers;

[Route("api/goals")]
public class GoalsController : BaseController
{
    public GoalsController(IMediator mediator) : base(mediator)
    {
    }

    /// <summary>
    /// Kullanıcının hedeflerini (kart listesi için, Aktif/Tamamlanan filtresiyle) getirmek için kullanılır
    /// </summary>
    /// <param name="request"></param>
    /// <returns></returns>
    [HttpGet]
    [ProducesResponseType(typeof(BaseResponse<PaginatedModel<GoalDto>>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(BaseResponse<object>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(BaseResponse<object>), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(BaseResponse<object>), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(BaseResponse<object>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> List([FromQuery] GetGoalsQuery request)
    {
        var response = await _mediator.Send(request);
        return Response(response);
    }

    /// <summary>
    /// Ekranda gösterilecek hedef metriklerini (aktif/tamamlanan sayısı, toplam hedef/biriken tutar) getirmek için kullanılır
    /// </summary>
    /// <param name="request"></param>
    /// <returns></returns>
    [HttpGet("metrics")]
    [ProducesResponseType(typeof(BaseResponse<GoalMetricsDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(BaseResponse<object>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(BaseResponse<object>), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(BaseResponse<object>), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(BaseResponse<object>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetMetrics([FromQuery] GetGoalMetricsQuery request)
    {
        var response = await _mediator.Send(request);
        return Response(response);
    }

    /// <summary>
    /// Spesifik bir hedefi getirmek için kullanılır
    /// </summary>
    /// <param name="goalId"></param>
    /// <returns></returns>
    [HttpGet("{goalId}")]
    [ProducesResponseType(typeof(BaseResponse<GoalDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(BaseResponse<object>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(BaseResponse<object>), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(BaseResponse<object>), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(BaseResponse<object>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Get(int goalId)
    {
        var request = new GetGoalQuery(goalId);
        var response = await _mediator.Send(request);
        return Response(response);
    }

    /// <summary>
    /// Hedef oluşturmak için kullanılır
    /// </summary>
    /// <param name="request"></param>
    /// <returns></returns>
    [HttpPost]
    [ProducesResponseType(typeof(BaseResponse<object>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(BaseResponse<object>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(BaseResponse<object>), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(BaseResponse<object>), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(BaseResponse<object>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Create([FromBody] CreateGoalCommand request)
    {
        var response = await _mediator.Send(request);
        return Response(response);
    }

    /// <summary>
    /// Spesifik bir hedefi güncellemek için kullanılır (mevcut biriken tutar buradan değiştirilemez)
    /// </summary>
    /// <param name="goalId"></param>
    /// <param name="request"></param>
    /// <returns></returns>
    [HttpPut("{goalId}")]
    [ProducesResponseType(typeof(BaseResponse<object>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(BaseResponse<object>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(BaseResponse<object>), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(BaseResponse<object>), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(BaseResponse<object>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Update(int goalId, [FromBody] UpdateGoalCommand request)
    {
        request.Id = goalId;
        var response = await _mediator.Send(request);
        return Response(response);
    }

    /// <summary>
    /// Spesifik bir hedefi silmek için kullanılır
    /// </summary>
    /// <param name="goalId"></param>
    /// <returns></returns>
    [HttpDelete("{goalId}")]
    [ProducesResponseType(typeof(BaseResponse<object>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(BaseResponse<object>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(BaseResponse<object>), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(BaseResponse<object>), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(BaseResponse<object>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(int goalId)
    {
        var request = new DeleteGoalCommand(goalId);
        var response = await _mediator.Send(request);
        return Response(response);
    }

    /// <summary>
    /// Hedefe katkı eklemek için kullanılır. Girilen tutar kadar Birikim tipinde gerçek bir işlem oluşturulur
    /// ve hedefin mevcut biriken tutarı (CurrentAmount) artırılır.
    /// </summary>
    /// <param name="goalId"></param>
    /// <param name="request"></param>
    /// <returns></returns>
    [HttpPost("{goalId}/contribute")]
    [ProducesResponseType(typeof(BaseResponse<object>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(BaseResponse<object>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(BaseResponse<object>), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(BaseResponse<object>), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(BaseResponse<object>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Contribute(int goalId, [FromBody] ContributeToGoalCommand request)
    {
        request.GoalId = goalId;
        var response = await _mediator.Send(request);
        return Response(response);
    }
}
