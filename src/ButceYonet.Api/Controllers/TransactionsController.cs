using ButceYonet.Application.Application.Features.Transactions.CreateTransaction;
using ButceYonet.Application.Application.Features.Transactions.DeleteTransaction;
using ButceYonet.Application.Application.Features.Transactions.GetTransaction;
using ButceYonet.Application.Application.Features.Transactions.GetTransactions;
using ButceYonet.Application.Application.Features.Transactions.UpdateTransaction;
using ButceYonet.Application.Application.Shared.Dtos;
using DotBoil.Entities;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace ButceYonet.Api.Controllers;

[Route("api/notebooks/{notebookId}/transactions")]
public class TransactionsController : BaseController
{
    public TransactionsController(IMediator mediator) 
        : base(mediator)
    {
    }

    /// <summary>
    /// İstenilen deftere ait gelir-gider kalemlerini getirir
    /// </summary>
    /// <param name="notebookId"></param>
    /// <param name="request"></param>
    /// <returns></returns>
    [HttpGet]
    [ProducesResponseType(typeof(BaseResponse<PaginatedModel<TransactionDto>>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(BaseResponse<object>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(BaseResponse<object>), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(BaseResponse<object>), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(BaseResponse<object>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> List(int notebookId, [FromQuery] GetTransactionsQuery request)
    {
        request.NotebookId = notebookId;
        var response = await _mediator.Send(request);
        return Response(response);
    }

    /// <summary>
    /// İstenilen deftere ait spesifik bir gelir-gider kalemini getirir
    /// </summary>
    /// <param name="notebookId"></param>
    /// <param name="transactionId"></param>
    /// <returns></returns>
    [HttpGet("{transactionId}")]
    [ProducesResponseType(typeof(BaseResponse<TransactionDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(BaseResponse<object>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(BaseResponse<object>), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(BaseResponse<object>), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(BaseResponse<object>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Get(int notebookId, int transactionId)
    {
        var request = new GetTransactionQuery(notebookId, transactionId);
        var response = await _mediator.Send(request);
        return Response(response);
    }

    /// <summary>
    /// İstenilen deftere gelir-gider kalemi eklemek için kullanılır
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
    public async Task<IActionResult> Create(int notebookId, [FromBody] CreateTransactionCommand request)
    {
        request.NotebookId = notebookId;
        var response = await _mediator.Send(request);
        return Response(response);
    }

    /// <summary>
    /// İstenilen deftere ait spesifik bir gelir-gider kalemini güncellemek için kullanılır
    /// </summary>
    /// <param name="notebookId"></param>
    /// <param name="request"></param>
    /// <returns></returns>
    [HttpPut]
    [ProducesResponseType(typeof(BaseResponse<object>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(BaseResponse<object>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(BaseResponse<object>), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(BaseResponse<object>), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(BaseResponse<object>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Update(int notebookId, [FromBody] UpdateTransactionCommand request)
    {
        request.NotebookId = notebookId;
        var response = await _mediator.Send(request);
        return Response(response);
    }

    /// <summary>
    /// İstenilen deftere ait spesifik bir gelir-gider kalemini silmek için kullanılır
    /// </summary>
    /// <param name="notebookId"></param>
    /// <param name="transactionId"></param>
    /// <returns></returns>
    [HttpDelete("{transactionId}")]
    public async Task<IActionResult> Delete(int notebookId, int transactionId)
    {
        var request = new DeleteTransactionCommand(notebookId, transactionId);
        var response = await _mediator.Send(request);
        return Response(response);
    }
}