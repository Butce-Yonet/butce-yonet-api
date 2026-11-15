using DotBoil.Entities;
using MediatR;

namespace ButceYonet.Application.Application.Features.Budgets.DeleteBudget;

public class DeleteBudgetCommand : IRequest<BaseResponse>
{
    public int BudgetId { get; set; }

    public DeleteBudgetCommand()
    {
    }

    public DeleteBudgetCommand(int budgetId)
    {
        BudgetId = budgetId;
    }
}
