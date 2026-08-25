using DotBoil.Entities;
using MediatR;

namespace ButceYonet.Application.Application.Features.Goals.ContributeToGoal;

public class ContributeToGoalCommand : IRequest<BaseResponse>
{
    public int GoalId { get; set; }
    public decimal Amount { get; set; }
    public DateTime? ContributionDate { get; set; }
}
