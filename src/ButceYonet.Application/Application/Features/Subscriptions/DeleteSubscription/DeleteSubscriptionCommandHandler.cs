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

namespace ButceYonet.Application.Application.Features.Subscriptions.DeleteSubscription;

public class DeleteSubscriptionCommandHandler : BaseHandler<DeleteSubscriptionCommand, BaseResponse>
{
    private readonly IRepository<Subscription, ButceYonetDbContext> _subscriptionRepository;

    public DeleteSubscriptionCommandHandler(
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

    public override async Task<BaseResponse> ExecuteRequest(DeleteSubscriptionCommand request, CancellationToken cancellationToken)
    {
        var subscription = await
            _subscriptionRepository
                .Get()
                .Where(s =>
                    s.UserId == _user.Id &&
                    s.Id == request.SubscriptionId)
                .FirstOrDefaultAsync(cancellationToken);

        if (subscription is null)
            throw new NotFoundException(typeof(Subscription));

        subscription.IsDeleted = true;
        _subscriptionRepository.Update(subscription);
        await _subscriptionRepository.SaveChangesAsync();

        return BaseResponse.Response(new { }, HttpStatusCode.OK);
    }
}
