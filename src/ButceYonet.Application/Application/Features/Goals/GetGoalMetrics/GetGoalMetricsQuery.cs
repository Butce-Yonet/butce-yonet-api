using DotBoil.Entities;
using MediatR;

namespace ButceYonet.Application.Application.Features.Goals.GetGoalMetrics;

/// <summary>
/// Hedef metrikleri: aktif/tamamlanan hedef sayısı, toplam hedef tutarı, toplam biriken tutar.
/// </summary>
public class GetGoalMetricsQuery : IRequest<BaseResponse>
{
    /// <summary>Belirtilirse sadece bu para birimindeki hedefler dahil edilir; null ise tüm para birimleri toplanır.</summary>
    public int? CurrencyId { get; set; }
}
