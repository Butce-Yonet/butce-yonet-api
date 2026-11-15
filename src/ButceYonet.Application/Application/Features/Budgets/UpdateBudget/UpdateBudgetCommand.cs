using DotBoil.Entities;
using MediatR;

namespace ButceYonet.Application.Application.Features.Budgets.UpdateBudget;

/// <summary>
/// Etiket ve para birimi değiştirilemez (geçmiş sinyallerin ve dönem geçmişinin anlamı bozulmasın diye);
/// gerekiyorsa bütçe silinip yeniden oluşturulur.
/// </summary>
public class UpdateBudgetCommand : IRequest<BaseResponse>
{
    public int Id { get; set; }
    public decimal Amount { get; set; }
    public int PeriodStartDay { get; set; } = 1;
}
