using DotBoil.Entities;
using MediatR;

namespace ButceYonet.Application.Application.Features.Budgets.GetBudgets;

public class GetBudgetsQuery : IRequest<BaseResponse>
{
    /// <summary>Belirtilirse sadece o para birimindeki bütçeler döner</summary>
    public int? CurrencyId { get; set; }
}
