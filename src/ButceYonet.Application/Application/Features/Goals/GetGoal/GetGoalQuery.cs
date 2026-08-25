using DotBoil.Entities;
using MediatR;

namespace ButceYonet.Application.Application.Features.Goals.GetGoal;

public class GetGoalQuery : IRequest<BaseResponse>
{
    public int GoalId { get; set; }

    public GetGoalQuery()
    {
    }

    public GetGoalQuery(int goalId)
    {
        GoalId = goalId;
    }
}
