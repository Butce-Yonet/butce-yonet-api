using DotBoil.Entities;
using MediatR;

namespace ButceYonet.Application.Application.Features.Goals.UpdateGoal;

public class UpdateGoalCommand : IRequest<BaseResponse>
{
    public int Id { get; set; }
    public string Name { get; set; }
    public decimal TargetAmount { get; set; }
    public int CurrencyId { get; set; }
    public DateTime? Deadline { get; set; }
    public List<int> Labels { get; set; }

    public UpdateGoalCommand()
    {
        Labels = new List<int>();
    }
}
