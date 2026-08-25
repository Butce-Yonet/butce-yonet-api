using DotBoil.Entities;
using MediatR;

namespace ButceYonet.Application.Application.Features.Goals.GetGoals;

public class GetGoalsQuery : PaginationFilter, IRequest<BaseResponse>
{
    /// <summary>Belirtilmezse (Tümü) hem aktif hem tamamlanmış hedefler döner</summary>
    public bool? IsCompleted { get; set; }
}
