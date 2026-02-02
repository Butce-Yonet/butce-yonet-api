using DotBoil.Entities;
using MediatR;

namespace ButceYonet.Application.Application.Features.PeriodSummaryReports.GetPeriodSummaryReport;

/// <summary>
/// Dönem özeti raporu: ay / hafta / özel aralık için toplam gelir, gider, net bakiye ve önceki dönemle kıyas.
/// </summary>
public class GetPeriodSummaryReportQuery : IRequest<BaseResponse>
{
    public int NotebookId { get; set; }
    /// <summary>Dönem başlangıç tarihi (dahil)</summary>
    public DateTime StartDate { get; set; }
    /// <summary>Dönem bitiş tarihi (dahil)</summary>
    public DateTime EndDate { get; set; }
    /// <summary>Belirtilirse sadece bu para birimindeki tutarlar dahil edilir; null ise tüm para birimleri toplanır.</summary>
    public int? CurrencyId { get; set; }
}
