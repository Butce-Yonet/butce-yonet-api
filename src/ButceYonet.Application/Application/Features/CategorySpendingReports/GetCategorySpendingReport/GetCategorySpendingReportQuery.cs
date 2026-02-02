using DotBoil.Entities;
using MediatR;

namespace ButceYonet.Application.Application.Features.CategorySpendingReports.GetCategorySpendingReport;

/// <summary>
/// En çok neye para harcıyorum raporu:
/// Seçilen dönem için kategori bazlı gider dağılımı.
/// </summary>
public class GetCategorySpendingReportQuery : IRequest<BaseResponse>
{
    public int NotebookId { get; set; }

    /// <summary>Dönem başlangıç tarihi (dahil)</summary>
    public DateTime StartDate { get; set; }

    /// <summary>Dönem bitiş tarihi (dahil)</summary>
    public DateTime EndDate { get; set; }

    /// <summary>
    /// Belirtilirse sadece bu para birimindeki giderler dahil edilir;
    /// null ise tüm para birimleri toplanır.
    /// </summary>
    public int? CurrencyId { get; set; }
}

