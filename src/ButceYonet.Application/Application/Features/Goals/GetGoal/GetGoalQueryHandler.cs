using System.Net;
using AutoMapper;
using ButceYonet.Application.Application.Interfaces;
using ButceYonet.Application.Application.Shared;
using ButceYonet.Application.Application.Shared.Dtos;
using ButceYonet.Application.Domain.Entities;
using ButceYonet.Application.Domain.Exceptions;
using ButceYonet.Application.Infrastructure.Data;
using DotBoil.Caching;
using DotBoil.EFCore;
using DotBoil.Entities;
using DotBoil.Localization;
using DotBoil.Parameter;
using Microsoft.EntityFrameworkCore;

namespace ButceYonet.Application.Application.Features.Goals.GetGoal;

public class GetGoalQueryHandler : BaseHandler<GetGoalQuery, BaseResponse>
{
    private readonly IRepository<Goal, ButceYonetDbContext> _goalRepository;

    public GetGoalQueryHandler(
        ICache cache,
        IUser user,
        IMapper mapper,
        ILocalize localize,
        IParameterManager parameter,
        IUserPlanValidator userPlanValidator,
        IRepository<Goal, ButceYonetDbContext> goalRepository)
        : base(cache, user, mapper, localize, parameter, userPlanValidator)
    {
        _goalRepository = goalRepository;
    }

    public override async Task<BaseResponse> ExecuteRequest(GetGoalQuery request, CancellationToken cancellationToken)
    {
        var goal = await _goalRepository
            .Get()
            .Where(g => g.Id == request.GoalId && g.UserId == _user.Id)
            .Include(g => g.Currency)
            .Include(g => g.GoalLabels.Where(gl => !gl.IsDeleted))
            .ThenInclude(gl => gl.UserLabel)
            .FirstOrDefaultAsync(cancellationToken);

        if (goal is null)
            throw new NotFoundException(typeof(Goal));

        var goalDto = _mapper.Map<GoalDto>(goal);
        GoalProgressCalculator.Apply(goalDto);

        return BaseResponse.Response(goalDto, HttpStatusCode.OK);
    }
}
