using DotBoil.Entities;
using MediatR;

namespace ButceYonet.Application.Application.Features.Transactions.GetCalendar;

/// <summary>
/// Belirli bir ay için takvim görünümü: her güne ait gelir/gider özeti ve önizleme işlemleri.
/// </summary>
public class GetCalendarQuery : IRequest<BaseResponse>
{
    public int Year { get; set; }
    public int Month { get; set; }

    /// <summary>Her gün için takvimde gösterilecek önizleme işlemi sayısı</summary>
    public int PreviewCount { get; set; } = 3;
}
