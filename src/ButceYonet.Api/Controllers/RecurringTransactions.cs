using ButceYonet.Application.Application.Features.RecurringTransactions.CreateRecurringTransaction;
using ButceYonet.Application.Application.Features.RecurringTransactions.DeleteRecurringTransaction;
using ButceYonet.Application.Application.Features.RecurringTransactions.GetRecurringTransaction;
using ButceYonet.Application.Application.Features.RecurringTransactions.GetRecurringTransactions;
using ButceYonet.Application.Application.Features.RecurringTransactions.UpdateRecurringTransaction;
using ButceYonet.Application.Application.Shared.Dtos;
using DotBoil.Entities;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace ButceYonet.Api.Controllers;

[Route("api/notebooks/{notebookId}/recurringtransactions")]
public class RecurringTransactions : BaseController
{
    public RecurringTransactions(IMediator mediator) : base(mediator)
    {
    }

    /// <summary>
    /// Zamanlanmış gelir-gider kayıtlarını getirmek için kullanılır
    /// </summary>
    /// <param name="notebookId"></param>
    /// <param name="request"></param>
    /// <returns></returns>
    [HttpGet]
    [ProducesResponseType(typeof(BaseResponse<PaginatedModel<RecurringTransactionDto>>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(BaseResponse<object>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(BaseResponse<object>), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(BaseResponse<object>), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(BaseResponse<object>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> List(int notebookId, [FromQuery]GetRecurringTransactionsQuery request)
    {
        request.NotebookId = notebookId;
        var response = await _mediator.Send(request);
        return Response(response);
    }

    /// <summary>
    /// Zamanlamış gelir-gider kayıdını getirmek için kullanılır
    /// </summary>
    /// <param name="notebookId"></param>
    /// <param name="recurringTransactionId"></param>
    /// <returns></returns>
    [HttpGet("{recurringTransactionId}")]
    [ProducesResponseType(typeof(BaseResponse<RecurringTransactionDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(BaseResponse<object>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(BaseResponse<object>), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(BaseResponse<object>), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(BaseResponse<object>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Get(int notebookId, int recurringTransactionId)
    {
        var request = new GetRecurringTransactionQuery(notebookId, recurringTransactionId);
        var response = await _mediator.Send(request);
        return Response(response);
    }

    /// <summary>
    /// Zamanlanmış gelir-gider oluşturmak için kullanılır
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
    public async Task<IActionResult> Create(int notebookId,
        [FromBody] CreateRecurringTransactionCommand request)
    {
        request.NotebookId = notebookId;
        var response = await _mediator.Send(request);
        return Response(response);
    }

    /// <summary>
    /// Zamanlanmış gelir-gider güncellemek için kullanılır
    /// </summary>
    /// <param name="notebookId"></param>
    /// <param name="recurringTransactionId"></param>
    /// <param name="request"></param>
    /// <returns></returns>
    [HttpPut("{recurringTransactionId}")]
    [ProducesResponseType(typeof(BaseResponse<object>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(BaseResponse<object>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(BaseResponse<object>), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(BaseResponse<object>), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(BaseResponse<object>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Update(int notebookId, int recurringTransactionId,
        [FromBody] UpdateRecurringTransactionCommand request)
    {
        request.Id = recurringTransactionId;
        request.NotebookId = notebookId;
        var response = await _mediator.Send(request);
        return Response(response);
    }

    /// <summary>
    /// Zamanlanmış gelir-gider bilgisini silmek için kullanılır
    /// </summary>
    /// <param name="notebookId"></param>
    /// <param name="recurringTransactionId"></param>
    /// <returns></returns>
    [HttpDelete("{recurringTransactionId}")]
    [ProducesResponseType(typeof(BaseResponse<object>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(BaseResponse<object>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(BaseResponse<object>), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(BaseResponse<object>), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(BaseResponse<object>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(int notebookId, int recurringTransactionId)
    {
        var deleteRecurringTransactionCommand = new DeleteRecurringTransactionCommand(notebookId, recurringTransactionId);
        var response = await _mediator.Send(deleteRecurringTransactionCommand);
        return Response(response);
    }
}