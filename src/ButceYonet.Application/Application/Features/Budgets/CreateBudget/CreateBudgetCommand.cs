using DotBoil.Entities;
using MediatR;

namespace ButceYonet.Application.Application.Features.Budgets.CreateBudget;

public class CreateBudgetCommand : IRequest<BaseResponse>
{
    /// <summary>Boş bırakılırsa etiketten bağımsız genel (tüm giderler) bütçesi oluşturulur</summary>
    public int? UserLabelId { get; set; }
    public decimal Amount { get; set; }
    public int CurrencyId { get; set; }

    /// <summary>Dönemin başladığı ayın günü (1-28), varsayılan 1</summary>
    public int PeriodStartDay { get; set; } = 1;
}
