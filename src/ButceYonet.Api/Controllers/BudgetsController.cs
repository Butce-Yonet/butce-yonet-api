using ButceYonet.Application.Application.Features.Budgets.CreateBudget;
using ButceYonet.Application.Application.Features.Budgets.DeleteBudget;
using ButceYonet.Application.Application.Features.Budgets.GetBudget;
using ButceYonet.Application.Application.Features.Budgets.GetBudgets;
using ButceYonet.Application.Application.Features.Budgets.GetBudgetSummary;
using ButceYonet.Application.Application.Features.Budgets.UpdateBudget;
using ButceYonet.Application.Application.Shared.Dtos;
using DotBoil.Entities;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace ButceYonet.Api.Controllers;

[Route("api/budgets")]
public class BudgetsController : BaseController
{
    public BudgetsController(IMediator mediator) : base(mediator)
    {
    }

    /// <summary>
    /// Kullanıcının bütçelerini aktif dönem durumlarıyla (harcanan, kalan, yüzde, durum, günlük harcanabilir) getirmek için kullanılır
    /// </summary>
    /// <param name="request"></param>
    /// <returns></returns>
    [HttpGet]
    [ProducesResponseType(typeof(BaseResponse<List<BudgetDto>>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(BaseResponse<object>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(BaseResponse<object>), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(BaseResponse<object>), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(BaseResponse<object>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> List([FromQuery] GetBudgetsQuery request)
    {
        var response = await _mediator.Send(request);
        return Response(response);
    }

    /// <summary>
    /// Para birimi bazında aktif dönem bütçe özetini (toplam limit/harcama, durum sayıları) getirmek için kullanılır
    /// </summary>
    /// <returns></returns>
    [HttpGet("summary")]
    [ProducesResponseType(typeof(BaseResponse<List<BudgetSummaryDto>>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(BaseResponse<object>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(BaseResponse<object>), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(BaseResponse<object>), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(BaseResponse<object>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetSummary()
    {
        var response = await _mediator.Send(new GetBudgetSummaryQuery());
        return Response(response);
    }

    /// <summary>
    /// Spesifik bir bütçeyi aktif dönem durumu ve son dönemlerin harcama geçmişiyle getirmek için kullanılır
    /// </summary>
    /// <param name="budgetId"></param>
    /// <param name="historyCount">Aktif dönem dahil geçmiş dönem sayısı (1-12, varsayılan 6)</param>
    /// <returns></returns>
    [HttpGet("{budgetId}")]
    [ProducesResponseType(typeof(BaseResponse<BudgetDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(BaseResponse<object>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(BaseResponse<object>), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(BaseResponse<object>), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(BaseResponse<object>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Get(int budgetId, [FromQuery] int historyCount = 6)
    {
        var request = new GetBudgetQuery(budgetId, historyCount);
        var response = await _mediator.Send(request);
        return Response(response);
    }

    /// <summary>
    /// Aylık bütçe oluşturmak için kullanılır. UserLabelId boş bırakılırsa genel (tüm giderler) bütçesi oluşturulur.
    /// Aynı etiket (ya da genel) ve para birimi için yalnızca bir bütçe tanımlanabilir.
    /// </summary>
    /// <param name="request"></param>
    /// <returns></returns>
    [HttpPost]
    [ProducesResponseType(typeof(BaseResponse<object>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(BaseResponse<object>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(BaseResponse<object>), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(BaseResponse<object>), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(BaseResponse<object>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Create([FromBody] CreateBudgetCommand request)
    {
        var response = await _mediator.Send(request);
        return Response(response);
    }

    /// <summary>
    /// Spesifik bir bütçenin limitini ve dönem başlangıç gününü güncellemek için kullanılır (etiket ve para birimi değiştirilemez)
    /// </summary>
    /// <param name="budgetId"></param>
    /// <param name="request"></param>
    /// <returns></returns>
    [HttpPut("{budgetId}")]
    [ProducesResponseType(typeof(BaseResponse<object>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(BaseResponse<object>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(BaseResponse<object>), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(BaseResponse<object>), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(BaseResponse<object>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Update(int budgetId, [FromBody] UpdateBudgetCommand request)
    {
        request.Id = budgetId;
        var response = await _mediator.Send(request);
        return Response(response);
    }

    /// <summary>
    /// Spesifik bir bütçeyi silmek için kullanılır
    /// </summary>
    /// <param name="budgetId"></param>
    /// <returns></returns>
    [HttpDelete("{budgetId}")]
    [ProducesResponseType(typeof(BaseResponse<object>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(BaseResponse<object>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(BaseResponse<object>), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(BaseResponse<object>), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(BaseResponse<object>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(int budgetId)
    {
        var request = new DeleteBudgetCommand(budgetId);
        var response = await _mediator.Send(request);
        return Response(response);
    }
}
