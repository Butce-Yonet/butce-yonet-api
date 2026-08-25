using System.Net;
using AutoMapper;
using ButceYonet.Application.Application.Interfaces;
using ButceYonet.Application.Domain.Entities;
using ButceYonet.Application.Infrastructure.Data;
using DotBoil.Caching;
using DotBoil.EFCore;
using DotBoil.Entities;
using DotBoil.Localization;
using DotBoil.Parameter;
using Microsoft.EntityFrameworkCore;

namespace ButceYonet.Application.Application.Features.Goals.CreateGoal;

public class CreateGoalCommandHandler : BaseHandler<CreateGoalCommand, BaseResponse>
{
    private readonly IRepository<UserLabel, ButceYonetDbContext> _userLabelRepository;
    private readonly IRepository<Goal, ButceYonetDbContext> _goalRepository;

    public CreateGoalCommandHandler(
        ICache cache,
        IUser user,
        IMapper mapper,
        ILocalize localize,
        IParameterManager parameter,
        IUserPlanValidator userPlanValidator,
        IRepository<UserLabel, ButceYonetDbContext> userLabelRepository,
        IRepository<Goal, ButceYonetDbContext> goalRepository)
        : base(cache, user, mapper, localize, parameter, userPlanValidator)
    {
        _userLabelRepository = userLabelRepository;
        _goalRepository = goalRepository;
    }

    public override async Task<BaseResponse> ExecuteRequest(CreateGoalCommand request, CancellationToken cancellationToken)
    {
        var userLabels = await _userLabelRepository
            .GetAll()
            .Where(ul => ul.UserId == null || ul.UserId == _user.Id)
            .ToListAsync(cancellationToken);

        var matchingLabelIds = userLabels
            .Where(ul => request.Labels.Contains(ul.Id))
            .Select(ul => ul.Id)
            .ToList();

        var goal = new Goal
        {
            UserId = _user.Id,
            Name = request.Name,
            TargetAmount = request.TargetAmount,
            CurrentAmount = 0,
            CurrencyId = request.CurrencyId,
            Deadline = request.Deadline,
            GoalLabels = matchingLabelIds.Select(id => new GoalLabel
            {
                UserLabelId = id
            }).ToList()
        };

        await _goalRepository.AddAsync(goal);
        await _goalRepository.SaveChangesAsync();

        return BaseResponse.Response(new { }, HttpStatusCode.OK);
    }
}
