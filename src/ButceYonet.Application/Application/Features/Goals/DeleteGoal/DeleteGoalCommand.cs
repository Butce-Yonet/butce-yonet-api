using DotBoil.Entities;
using MediatR;

namespace ButceYonet.Application.Application.Features.Goals.DeleteGoal;

public class DeleteGoalCommand : IRequest<BaseResponse>
{
    public int GoalId { get; set; }

    public DeleteGoalCommand()
    {
    }

    public DeleteGoalCommand(int goalId)
    {
        GoalId = goalId;
    }
}
