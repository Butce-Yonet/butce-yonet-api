using System.Net;
using AutoMapper;
using ButceYonet.Application.Application.Interfaces;
using ButceYonet.Application.Application.Shared.Dtos;
using ButceYonet.Application.Domain.Entities;
using ButceYonet.Application.Infrastructure.Data;
using DotBoil.Caching;
using DotBoil.EFCore;
using DotBoil.EFCore.Extensions;
using DotBoil.Entities;
using DotBoil.Localization;
using DotBoil.Parameter;
using Microsoft.EntityFrameworkCore;

namespace ButceYonet.Application.Application.Features.Goals.GetGoalMetrics;

public class GetGoalMetricsQueryHandler : BaseHandler<GetGoalMetricsQuery, BaseResponse>
{
    private readonly IRepository<Goal, ButceYonetDbContext> _goalRepository;
    private readonly IRepository<Currency, ButceYonetDbContext> _currencyRepository;

    public GetGoalMetricsQueryHandler(
        ICache cache,
        IUser user,
        IMapper mapper,
        ILocalize localize,
        IParameterManager parameter,
        IUserPlanValidator userPlanValidator,
        IRepository<Goal, ButceYonetDbContext> goalRepository,
        IRepository<Currency, ButceYonetDbContext> currencyRepository)
        : base(cache, user, mapper, localize, parameter, userPlanValidator)
    {
        _goalRepository = goalRepository;
        _currencyRepository = currencyRepository;
    }

    public override async Task<BaseResponse> ExecuteRequest(GetGoalMetricsQuery request, CancellationToken cancellationToken)
    {
        var goals = await _goalRepository
            .GetAll()
            .Where(g => g.UserId == _user.Id)
            .WhereIf(request.CurrencyId.HasValue, g => g.CurrencyId == request.CurrencyId)
            .ToListAsync(cancellationToken);

        CurrencyDto currencyDto = null;
        if (request.CurrencyId.HasValue)
        {
            var currency = await _currencyRepository.Get()
                .Where(c => c.Id == request.CurrencyId.Value)
                .FirstOrDefaultAsync(cancellationToken);
            if (currency != null)
                currencyDto = _mapper.Map<CurrencyDto>(currency);
        }

        var dto = new GoalMetricsDto
        {
            ActiveGoalCount = goals.Count(g => g.CurrentAmount < g.TargetAmount),
            CompletedGoalCount = goals.Count(g => g.CurrentAmount >= g.TargetAmount),
            TotalTargetAmount = goals.Sum(g => g.TargetAmount),
            TotalSavedAmount = goals.Sum(g => g.CurrentAmount),
            Currency = currencyDto
        };

        return BaseResponse.Response(dto, HttpStatusCode.OK);
    }
}
