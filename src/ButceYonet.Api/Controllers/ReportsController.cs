using ButceYonet.Application.Application.Features.CategorizedTransactionReports.GetCategorizedTransactionReport;
using ButceYonet.Application.Application.Features.NonCategorizedTransactionReports.GetNonCategorizedTransactionReport;
using ButceYonet.Application.Application.Features.PeriodSummaryReports.GetPeriodSummaryReport;
using ButceYonet.Application.Application.Features.CategorySpendingReports.GetCategorySpendingReport;
using ButceYonet.Application.Application.Shared.Dtos;
using DotBoil.Entities;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace ButceYonet.Api.Controllers;

public class ReportsController : BaseController
{
    public ReportsController(IMediator mediator) 
        : base(mediator)
    {
    }

    /// <summary>
    /// Belirli tarihler aralarında kategorize edilmiş şekilde gelir-giderleri rapor olarak almak için kullanılır.
    /// </summary>
    /// <returns></returns>
    [HttpGet("transactions/categorized")]
    [ProducesResponseType(typeof(BaseResponse<List<CategorizedTransactionReportDto>>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(BaseResponse<List<CategorizedTransactionReportDto>>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(BaseResponse<List<CategorizedTransactionReportDto>>), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(BaseResponse<List<CategorizedTransactionReportDto>>), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(BaseResponse<List<CategorizedTransactionReportDto>>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetTransactionCategories([FromQuery] GetCategorizedTransactionReportQuery request)
    {
        var response = await _mediator.Send(request);

        return Response(response);
    }

    /// <summary>
    /// Belirli tarihler aralarında kategorize edilmemiş şekilde gelir giderleri rapor olarak almak için kullanılır.
    /// </summary>
    /// <param name="request"></param>
    /// <returns></returns>
    [HttpGet("transactions/noncategorized")]
    [ProducesResponseType(typeof(BaseResponse<List<NonCategorizedTransactionReportDto>>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(BaseResponse<List<NonCategorizedTransactionReportDto>>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(BaseResponse<List<NonCategorizedTransactionReportDto>>), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(BaseResponse<List<NonCategorizedTransactionReportDto>>), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(BaseResponse<List<NonCategorizedTransactionReportDto>>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetTransactionCategoriesNoncategorized([FromQuery] GetNonCategorizedTransactionReportQuery request)
    {
        var response = await _mediator.Send(request);
        
        return Response(response);
    }

    /// <summary>
    /// Dönem özeti raporu: seçilen dönem (ay / hafta / özel aralık) için toplam gelir, toplam gider, net bakiye ve önceki dönemle kıyas (% artış/azalış).
    /// </summary>
    /// <param name="request">NotebookId, StartDate, EndDate, isteğe bağlı CurrencyId</param>
    /// <returns>PeriodSummaryReportDto</returns>
    [HttpGet("period-summary")]
    [ProducesResponseType(typeof(BaseResponse<PeriodSummaryReportDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(BaseResponse<object>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(BaseResponse<object>), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(BaseResponse<object>), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(BaseResponse<object>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetPeriodSummary([FromQuery] GetPeriodSummaryReportQuery request)
    {
        var response = await _mediator.Send(request);
        return Response(response);
    }

    /// <summary>
    /// En çok neye para harcıyorum? Seçilen dönem için kategori bazlı gider dağılımı
    /// (toplam tutar, toplam harcama içindeki yüzdelik pay ve önceki dönem toplamı).
    /// </summary>
    /// <param name="request">NotebookId, StartDate, EndDate, isteğe bağlı CurrencyId</param>
    /// <returns>CategorySpendingReportDto</returns>
    [HttpGet("category-spending")]
    [ProducesResponseType(typeof(BaseResponse<CategorySpendingReportDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(BaseResponse<object>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(BaseResponse<object>), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(BaseResponse<object>), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(BaseResponse<object>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetCategorySpending([FromQuery] GetCategorySpendingReportQuery request)
    {
        var response = await _mediator.Send(request);
        return Response(response);
    }
}