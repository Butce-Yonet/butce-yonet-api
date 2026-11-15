using DotBoil.Entities;
using MediatR;

namespace ButceYonet.Application.Application.Features.Budgets.GetBudget;

public class GetBudgetQuery : IRequest<BaseResponse>
{
    public int BudgetId { get; set; }

    /// <summary>Aktif dönem dahil kaç dönemlik geçmiş dönülsün (1-12, varsayılan 6)</summary>
    public int HistoryCount { get; set; } = 6;

    public GetBudgetQuery()
    {
    }

    public GetBudgetQuery(int budgetId, int historyCount)
    {
        BudgetId = budgetId;
        HistoryCount = historyCount;
    }
}
