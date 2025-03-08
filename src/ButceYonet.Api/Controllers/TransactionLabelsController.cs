using ButceYonet.Application.Application.Features.Transactions.AddTransactionLabel;
using ButceYonet.Application.Application.Features.Transactions.RemoveTransactionLabel;
using DotBoil.Entities;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace ButceYonet.Api.Controllers;

[Route("api/notebooks/{notebookId}/transactions/{transactionId}/labels")]
public class TransactionLabelsController : BaseController
{
    public TransactionLabelsController(IMediator mediator) : base(mediator)
    {
    }

    /// <summary>
    /// İstenilen defterde spesifik bir gelir-gider kalemine kategori eklemek için kullanılır.
    /// </summary>
    /// <param name="notebookId"></param>
    /// <param name="transactionId"></param>
    /// <param name="labelId"></param>
    /// <returns></returns>
    [HttpPost("{labelId}")]
    [ProducesResponseType(typeof(BaseResponse<object>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(BaseResponse<object>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(BaseResponse<object>), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(BaseResponse<object>), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(BaseResponse<object>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Create(int notebookId, int transactionId, int labelId)
    {
        var request = new AddTransactionLabelCommand(notebookId, transactionId, labelId);
        var response = await _mediator.Send(request);
        return Response(response);
    }

    /// <summary>
    /// İstenilen defterde spesifik bir gelir-gider kalemine kategori silmek için kullanılır.
    /// </summary>
    /// <param name="notebookId"></param>
    /// <param name="transactionId"></param>
    /// <param name="labelId"></param>
    /// <returns></returns>
    [HttpDelete("{labelId}")]
    [ProducesResponseType(typeof(BaseResponse<object>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(BaseResponse<object>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(BaseResponse<object>), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(BaseResponse<object>), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(BaseResponse<object>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(int notebookId, int transactionId, int labelId)
    {
        var request = new RemoveTransactionLabelCommand(notebookId, transactionId, labelId);
        var response = await _mediator.Send(request);
        return Response(response);
    }
}