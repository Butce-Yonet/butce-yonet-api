using System.Net;
using AutoMapper;
using ButceYonet.Application.Application.Interfaces;
using ButceYonet.Application.Application.Shared.Dtos;
using ButceYonet.Application.Domain.Constants;
using ButceYonet.Application.Domain.Entities;
using ButceYonet.Application.Infrastructure.Data;
using DotBoil.Caching;
using DotBoil.EFCore;
using DotBoil.Entities;
using DotBoil.Localization;
using DotBoil.Parameter;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace ButceYonet.Application.Application.Features.Plans.GetPlans;

public class GetPlanQueryHandler : BaseQueryHandler<GetPlanQuery, BaseResponse>
{
    private readonly IRepository<Plan, ButceYonetDbContext> _planRepository;
    
    public GetPlanQueryHandler(
        ICache cache,
        IUser user,
        IMapper mapper,
        ILocalize localize, 
        IParameterManager parameter,
        IUserPlanValidator userPlanValidator,
        IRepository<Plan, ButceYonetDbContext> planRepository) 
        : base(cache, user, mapper, localize, parameter, userPlanValidator)
    {
        _planRepository = planRepository;
    }

    public override async Task<BaseResponse> ExecuteRequest(GetPlanQuery request, CancellationToken cancellationToken)
    {
        var plans = await _cache.GetOrSetAsync(CacheKeyConstants.Plans, async () =>
        {
            return await _planRepository
                .GetAll()
                .Include(p => p.PlanFeatures)
                .ToListAsync();
        }, CacheIntervalConstants.Plans);

        var responseModel = _mapper.Map<List<PlanDto>>(plans);

        return BaseResponse.Response(responseModel, HttpStatusCode.OK);
    }
}