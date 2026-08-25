using ButceYonet.Application.Application.Features.Subscriptions.CreateSubscription;
using ButceYonet.Application.Application.Features.Subscriptions.DeleteSubscription;
using ButceYonet.Application.Application.Features.Subscriptions.GetSubscription;
using ButceYonet.Application.Application.Features.Subscriptions.GetSubscriptions;
using ButceYonet.Application.Application.Features.Subscriptions.MarkSubscriptionAsPaid;
using ButceYonet.Application.Application.Features.Subscriptions.UpdateSubscription;
using ButceYonet.Application.Application.Shared.Dtos;
using DotBoil.Entities;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace ButceYonet.Api.Controllers;

[Route("api/subscriptions")]
public class SubscriptionsController : BaseController
{
    public SubscriptionsController(IMediator mediator) : base(mediator)
    {
    }

    /// <summary>
    /// Kullanıcının aboneliklerini (Tümü/Yaklaşan/Ödendi/Gecikmiş filtreleriyle) getirmek için kullanılır
    /// </summary>
    /// <param name="request"></param>
    /// <returns></returns>
    [HttpGet]
    [ProducesResponseType(typeof(BaseResponse<PaginatedModel<SubscriptionDto>>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(BaseResponse<object>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(BaseResponse<object>), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(BaseResponse<object>), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(BaseResponse<object>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> List([FromQuery] GetSubscriptionsQuery request)
    {
        var response = await _mediator.Send(request);
        return Response(response);
    }

    /// <summary>
    /// Spesifik bir aboneliği getirmek için kullanılır
    /// </summary>
    /// <param name="subscriptionId"></param>
    /// <returns></returns>
    [HttpGet("{subscriptionId}")]
    [ProducesResponseType(typeof(BaseResponse<SubscriptionDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(BaseResponse<object>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(BaseResponse<object>), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(BaseResponse<object>), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(BaseResponse<object>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Get(int subscriptionId)
    {
        var request = new GetSubscriptionQuery(subscriptionId);
        var response = await _mediator.Send(request);
        return Response(response);
    }

    /// <summary>
    /// Abonelik oluşturmak için kullanılır (sadece giderler için)
    /// </summary>
    /// <param name="request"></param>
    /// <returns></returns>
    [HttpPost]
    [ProducesResponseType(typeof(BaseResponse<object>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(BaseResponse<object>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(BaseResponse<object>), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(BaseResponse<object>), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(BaseResponse<object>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Create([FromBody] CreateSubscriptionCommand request)
    {
        var response = await _mediator.Send(request);
        return Response(response);
    }

    /// <summary>
    /// Spesifik bir aboneliği güncellemek için kullanılır
    /// </summary>
    /// <param name="subscriptionId"></param>
    /// <param name="request"></param>
    /// <returns></returns>
    [HttpPut("{subscriptionId}")]
    [ProducesResponseType(typeof(BaseResponse<object>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(BaseResponse<object>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(BaseResponse<object>), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(BaseResponse<object>), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(BaseResponse<object>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Update(int subscriptionId, [FromBody] UpdateSubscriptionCommand request)
    {
        request.Id = subscriptionId;
        var response = await _mediator.Send(request);
        return Response(response);
    }

    /// <summary>
    /// Spesifik bir aboneliği silmek için kullanılır
    /// </summary>
    /// <param name="subscriptionId"></param>
    /// <returns></returns>
    [HttpDelete("{subscriptionId}")]
    [ProducesResponseType(typeof(BaseResponse<object>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(BaseResponse<object>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(BaseResponse<object>), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(BaseResponse<object>), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(BaseResponse<object>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(int subscriptionId)
    {
        var request = new DeleteSubscriptionCommand(subscriptionId);
        var response = await _mediator.Send(request);
        return Response(response);
    }

    /// <summary>
    /// Aboneliği ödendi olarak işaretlemek için kullanılır. Gerçek tutarla bir Gider işlemi oluşturulur
    /// ve bir sonraki vade tarihi (NextOccurrence) ilerletilir.
    /// </summary>
    /// <param name="subscriptionId"></param>
    /// <param name="request"></param>
    /// <returns></returns>
    [HttpPost("{subscriptionId}/pay")]
    [ProducesResponseType(typeof(BaseResponse<object>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(BaseResponse<object>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(BaseResponse<object>), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(BaseResponse<object>), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(BaseResponse<object>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Pay(int subscriptionId, [FromBody] MarkSubscriptionAsPaidCommand request)
    {
        request.SubscriptionId = subscriptionId;
        var response = await _mediator.Send(request);
        return Response(response);
    }
}
