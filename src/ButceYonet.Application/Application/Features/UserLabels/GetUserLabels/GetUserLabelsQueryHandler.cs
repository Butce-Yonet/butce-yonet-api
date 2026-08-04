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
using Microsoft.EntityFrameworkCore;

namespace ButceYonet.Application.Application.Features.UserLabels.GetUserLabels;

public class GetUserLabelsQueryHandler : BaseHandler<GetUserLabelsQuery, BaseResponse>
{
    private readonly IRepository<UserLabel, ButceYonetDbContext> _userLabelRepository;

    public GetUserLabelsQueryHandler(
        ICache cache,
        IUser user,
        IMapper mapper,
        ILocalize localize,
        IParameterManager parameter,
        IUserPlanValidator userPlanValidator,
        IRepository<UserLabel, ButceYonetDbContext> userLabelRepository)
        : base(cache, user, mapper, localize, parameter, userPlanValidator)
    {
        _userLabelRepository = userLabelRepository;
    }

    public override async Task<BaseResponse> ExecuteRequest(GetUserLabelsQuery request, CancellationToken cancellationToken)
    {
        var cacheKey = CacheKeyConstants.UserLabels.Replace("{UserId}", _user.Id.ToString());

        var labels = await _cache.GetOrSetAsync(cacheKey, async () =>
        {
            var items = await _userLabelRepository
                .GetAll()
                .Where(ul => ul.UserId == null || ul.UserId == _user.Id)
                .OrderBy(ul => ul.Name)
                .ToListAsync();

            return _mapper.Map<List<UserLabelDto>>(items);
        }, CacheIntervalConstants.UserLabels);

        return BaseResponse.Response(labels, HttpStatusCode.OK);
    }
}