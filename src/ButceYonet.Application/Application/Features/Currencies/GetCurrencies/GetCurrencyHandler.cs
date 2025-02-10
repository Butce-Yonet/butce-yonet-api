using System.Net;
using AutoMapper;
using ButceYonet.Application.Application.Interfaces;
using ButceYonet.Application.Application.Shared.Dtos;
using ButceYonet.Application.Domain.Constants;
using ButceYonet.Application.Domain.Entities;
using ButceYonet.Application.Infrastructure.Data;
using DotBoil.Caching;
using DotBoil.EFCore;
using DotBoil.EFCore.Extensions;
using DotBoil.Entities;
using DotBoil.Localization;
using DotBoil.Parameter;
using Microsoft.EntityFrameworkCore;

namespace ButceYonet.Application.Application.Features.Currencies.GetCurrencies;

public class GetCurrencyHandler : BaseHandler<GetCurrencyQuery, BaseResponse>
{
    private readonly IRepository<Currency, ButceYonetDbContext> _currencyRepository;
    
    public GetCurrencyHandler(
        ICache cache,
        IUser user,
        IMapper mapper,
        ILocalize localize,
        IParameterManager parameter,
        IUserPlanValidator userPlanValidator,
        IRepository<Currency, ButceYonetDbContext> currencyRepository)
        : base(cache, user, mapper, localize, parameter, userPlanValidator)
    {
        _currencyRepository = currencyRepository;
    }

    public override async Task<BaseResponse> ExecuteRequest(GetCurrencyQuery request, CancellationToken cancellationToken)
    {
        var currencies = await _cache.GetOrSetAsync(CacheKeyConstants.Currencies, async () =>
        {
            return await _currencyRepository
                .GetAll()
                .ToListAsync();
        }, CacheIntervalConstants.Currencies);

        var responseModel = _mapper.Map<List<CurrencyDto>>(currencies);

        return BaseResponse.Response(responseModel, HttpStatusCode.OK);
    }
}