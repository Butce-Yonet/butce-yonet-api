using ButceYonet.Application.Application.Features.Transactions.CreateTransaction;
using ButceYonet.Application.Application.Features.Transactions.DeleteTransaction;
using ButceYonet.Application.Application.Features.Transactions.GetCalendar;
using ButceYonet.Application.Application.Features.Transactions.GetTransaction;
using ButceYonet.Application.Application.Features.Transactions.GetTransactions;
using ButceYonet.Application.Application.Features.Transactions.UpdateTransaction;
using ButceYonet.Application.Application.Shared.Dtos;
using DotBoil.Entities;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace ButceYonet.Api.Controllers;

[Route("api/transactions")]
public class TransactionsController : BaseController
{
    public TransactionsController(IMediator mediator)
        : base(mediator)
    {
    }

    /// <summary>
    /// Mevcut kullanıcıya ait gelir-gider kalemlerini getirir
    /// </summary>
    /// <param name="request"></param>
    /// <returns></returns>
    [HttpGet]
    [ProducesResponseType(typeof(BaseResponse<PaginatedModel<TransactionDto>>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(BaseResponse<object>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(BaseResponse<object>), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(BaseResponse<object>), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(BaseResponse<object>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> List([FromQuery] GetTransactionsQuery request)
    {
        var response = await _mediator.Send(request);
        return Response(response);
    }

    /// <summary>
    /// Belirli bir ay için takvim görünümü: her güne ait toplam gelir/gider ve önizleme işlemleri.
    /// Bir güne ait tüm işlemler için StartTime/EndTime filtresiyle transaction listesi ayrıca sorgulanmalıdır.
    /// </summary>
    /// <param name="request"></param>
    /// <returns></returns>
    [HttpGet("calendar")]
    [ProducesResponseType(typeof(BaseResponse<CalendarMonthDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(BaseResponse<object>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(BaseResponse<object>), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(BaseResponse<object>), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(BaseResponse<object>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetCalendar([FromQuery] GetCalendarQuery request)
    {
        var response = await _mediator.Send(request);
        return Response(response);
    }

    /// <summary>
    /// Spesifik bir gelir-gider kalemini getirir
    /// </summary>
    /// <param name="transactionId"></param>
    /// <returns></returns>
    [HttpGet("{transactionId}")]
    [ProducesResponseType(typeof(BaseResponse<TransactionDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(BaseResponse<object>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(BaseResponse<object>), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(BaseResponse<object>), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(BaseResponse<object>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Get(int transactionId)
    {
        var request = new GetTransactionQuery(transactionId);
        var response = await _mediator.Send(request);
        return Response(response);
    }

    /// <summary>
    /// Gelir-gider kalemi eklemek için kullanılır. İşlem tarihine göre ilgili aya ait defter otomatik bulunur/oluşturulur.
    /// </summary>
    /// <param name="request"></param>
    /// <returns></returns>
    [HttpPost]
    [ProducesResponseType(typeof(BaseResponse<object>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(BaseResponse<object>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(BaseResponse<object>), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(BaseResponse<object>), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(BaseResponse<object>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Create([FromBody] CreateTransactionCommand request)
    {
        var response = await _mediator.Send(request);
        return Response(response);
    }

    /// <summary>
    /// Spesifik bir gelir-gider kalemini güncellemek için kullanılır
    /// </summary>
    /// <param name="request"></param>
    /// <returns></returns>
    [HttpPut]
    [ProducesResponseType(typeof(BaseResponse<object>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(BaseResponse<object>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(BaseResponse<object>), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(BaseResponse<object>), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(BaseResponse<object>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Update([FromBody] UpdateTransactionCommand request)
    {
        var response = await _mediator.Send(request);
        return Response(response);
    }

    /// <summary>
    /// Spesifik bir gelir-gider kalemini silmek için kullanılır
    /// </summary>
    /// <param name="transactionId"></param>
    /// <returns></returns>
    [HttpDelete("{transactionId}")]
    public async Task<IActionResult> Delete(int transactionId)
    {
        var request = new DeleteTransactionCommand(transactionId);
        var response = await _mediator.Send(request);
        return Response(response);
    }
}
