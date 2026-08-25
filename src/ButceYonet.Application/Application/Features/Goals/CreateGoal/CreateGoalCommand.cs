using DotBoil.Entities;
using MediatR;

namespace ButceYonet.Application.Application.Features.Goals.CreateGoal;

public class CreateGoalCommand : IRequest<BaseResponse>
{
    public string Name { get; set; }
    public decimal TargetAmount { get; set; }
    public int CurrencyId { get; set; }
    public DateTime? Deadline { get; set; }
    public List<int> Labels { get; set; }

    public CreateGoalCommand()
    {
        Labels = new List<int>();
    }
}
