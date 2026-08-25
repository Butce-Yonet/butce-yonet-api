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

namespace ButceYonet.Application.Application.Features.Subscriptions.GetSubscription;

public class GetSubscriptionQueryHandler : BaseHandler<GetSubscriptionQuery, BaseResponse>
{
    private readonly IRepository<Subscription, ButceYonetDbContext> _subscriptionRepository;

    public GetSubscriptionQueryHandler(
        ICache cache,
        IUser user,
        IMapper mapper,
        ILocalize localize,
        IParameterManager parameter,
        IUserPlanValidator userPlanValidator,
        IRepository<Subscription, ButceYonetDbContext> subscriptionRepository)
        : base(cache, user, mapper, localize, parameter, userPlanValidator)
    {
        _subscriptionRepository = subscriptionRepository;
    }

    public override async Task<BaseResponse> ExecuteRequest(GetSubscriptionQuery request, CancellationToken cancellationToken)
    {
        var subscription = await _subscriptionRepository
            .Get()
            .Where(s => s.Id == request.SubscriptionId && s.UserId == _user.Id)
            .Include(s => s.Currency)
            .Include(s => s.SubscriptionLabels.Where(sl => !sl.IsDeleted))
            .ThenInclude(sl => sl.UserLabel)
            .FirstOrDefaultAsync(cancellationToken);

        if (subscription is null)
            throw new NotFoundException(typeof(Subscription));

        var subscriptionDto = _mapper.Map<SubscriptionDto>(subscription);
        subscriptionDto.Status = SubscriptionStatusCalculator.Calculate(subscription.NextOccurrence, subscription.LastPaidDate, DateTime.UtcNow.Date);

        return BaseResponse.Response(subscriptionDto, HttpStatusCode.OK);
    }
}
