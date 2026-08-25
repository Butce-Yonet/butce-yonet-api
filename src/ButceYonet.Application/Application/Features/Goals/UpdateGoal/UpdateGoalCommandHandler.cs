using System.Net;
using AutoMapper;
using ButceYonet.Application.Application.Interfaces;
using ButceYonet.Application.Domain.Entities;
using ButceYonet.Application.Domain.Exceptions;
using ButceYonet.Application.Infrastructure.Data;
using DotBoil.Caching;
using DotBoil.EFCore;
using DotBoil.Entities;
using DotBoil.Localization;
using DotBoil.Parameter;
using Microsoft.EntityFrameworkCore;

namespace ButceYonet.Application.Application.Features.Goals.UpdateGoal;

public class UpdateGoalCommandHandler : BaseHandler<UpdateGoalCommand, BaseResponse>
{
    private readonly IRepository<Goal, ButceYonetDbContext> _goalRepository;
    private readonly IRepository<GoalLabel, ButceYonetDbContext> _goalLabelRepository;
    private readonly IRepository<UserLabel, ButceYonetDbContext> _userLabelRepository;

    public UpdateGoalCommandHandler(
        ICache cache,
        IUser user,
        IMapper mapper,
        ILocalize localize,
        IParameterManager parameter,
        IUserPlanValidator userPlanValidator,
        IRepository<Goal, ButceYonetDbContext> goalRepository,
        IRepository<GoalLabel, ButceYonetDbContext> goalLabelRepository,
        IRepository<UserLabel, ButceYonetDbContext> userLabelRepository)
        : base(cache, user, mapper, localize, parameter, userPlanValidator)
    {
        _goalRepository = goalRepository;
        _goalLabelRepository = goalLabelRepository;
        _userLabelRepository = userLabelRepository;
    }

    public override async Task<BaseResponse> ExecuteRequest(UpdateGoalCommand request, CancellationToken cancellationToken)
    {
        var goal = await _goalRepository
            .Get()
            .Where(g => g.Id == request.Id && g.UserId == _user.Id)
            .Include(g => g.GoalLabels.Where(gl => !gl.IsDeleted))
            .FirstOrDefaultAsync(cancellationToken);

        if (goal is null)
            throw new NotFoundException(typeof(Goal));

        goal.Name = request.Name;
        goal.TargetAmount = request.TargetAmount;
        goal.CurrencyId = request.CurrencyId;
        goal.Deadline = request.Deadline;

        foreach (var label in goal.GoalLabels)
        {
            label.IsDeleted = true;
            _goalLabelRepository.Update(label);
        }

        var userLabels = await _userLabelRepository
            .GetAll()
            .Where(ul => ul.UserId == null || ul.UserId == _user.Id)
            .ToListAsync(cancellationToken);

        var matchingLabelIds = userLabels
            .Where(ul => request.Labels.Contains(ul.Id))
            .Select(ul => ul.Id)
            .ToList();

        foreach (var labelId in matchingLabelIds)
        {
            await _goalLabelRepository.AddAsync(new GoalLabel
            {
                GoalId = goal.Id,
                UserLabelId = labelId
            });
        }

        _goalRepository.Update(goal);
        await _goalRepository.SaveChangesAsync();

        return BaseResponse.Response(new { }, HttpStatusCode.OK);
    }
}
