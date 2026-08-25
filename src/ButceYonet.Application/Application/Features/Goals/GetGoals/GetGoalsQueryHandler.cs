using System.Net;
using AutoMapper;
using ButceYonet.Application.Application.Interfaces;
using ButceYonet.Application.Application.Shared;
using ButceYonet.Application.Application.Shared.Dtos;
using ButceYonet.Application.Domain.Entities;
using ButceYonet.Application.Infrastructure.Data;
using DotBoil.Caching;
using DotBoil.EFCore;
using DotBoil.EFCore.Extensions;
using DotBoil.Entities;
using DotBoil.Localization;
using DotBoil.Parameter;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace ButceYonet.Application.Application.Features.Goals.GetGoals;

public class GetGoalsQueryHandler : BaseHandler<GetGoalsQuery, BaseResponse>
{
    private readonly IRepository<Goal, ButceYonetDbContext> _goalRepository;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public GetGoalsQueryHandler(
        ICache cache,
        IUser user,
        IMapper mapper,
        ILocalize localize,
        IParameterManager parameter,
        IUserPlanValidator userPlanValidator,
        IRepository<Goal, ButceYonetDbContext> goalRepository,
        IHttpContextAccessor httpContextAccessor)
        : base(cache, user, mapper, localize, parameter, userPlanValidator)
    {
        _goalRepository = goalRepository;
        _httpContextAccessor = httpContextAccessor;
    }

    public override async Task<BaseResponse> ExecuteRequest(GetGoalsQuery request, CancellationToken cancellationToken)
    {
        var paginationRequest = new PaginationFilter(
            int.Parse(_httpContextAccessor.HttpContext.Request.Query["PageNumber"].ToString()),
            int.Parse(_httpContextAccessor.HttpContext.Request.Query["PageSize"].ToString()));

        var query = _goalRepository
            .GetAll()
            .Where(g => g.UserId == _user.Id)
            .WhereIf(request.IsCompleted == true, g => g.CurrentAmount >= g.TargetAmount)
            .WhereIf(request.IsCompleted == false, g => g.CurrentAmount < g.TargetAmount);

        var goals = await query
            .Include(g => g.Currency)
            .Include(g => g.GoalLabels.Where(gl => !gl.IsDeleted))
            .ThenInclude(gl => gl.UserLabel)
            .OrderBy(g => g.Deadline ?? DateTime.MaxValue)
            .PaginateAsync(paginationRequest);

        var goalDtos = _mapper.Map<List<GoalDto>>(goals.Items);
        foreach (var dto in goalDtos)
            GoalProgressCalculator.Apply(dto);

        var paginatedResponse = new PaginatedModel<GoalDto>(
            goals.PageNumber,
            goals.PageSize,
            goals.TotalPages,
            goals.TotalRecords,
            goalDtos);

        return BaseResponse.Response(paginatedResponse, HttpStatusCode.OK);
    }
}
