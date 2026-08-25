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

namespace ButceYonet.Application.Application.Features.Subscriptions.CreateSubscription;

public class CreateSubscriptionCommandHandler : BaseHandler<CreateSubscriptionCommand, BaseResponse>
{
    private readonly IRepository<UserLabel, ButceYonetDbContext> _userLabelRepository;
    private readonly IRepository<Subscription, ButceYonetDbContext> _subscriptionRepository;

    public CreateSubscriptionCommandHandler(
        ICache cache,
        IUser user,
        IMapper mapper,
        ILocalize localize,
        IParameterManager parameter,
        IUserPlanValidator userPlanValidator,
        IRepository<UserLabel, ButceYonetDbContext> userLabelRepository,
        IRepository<Subscription, ButceYonetDbContext> subscriptionRepository)
        : base(cache, user, mapper, localize, parameter, userPlanValidator)
    {
        _userLabelRepository = userLabelRepository;
        _subscriptionRepository = subscriptionRepository;
    }

    public override async Task<BaseResponse> ExecuteRequest(CreateSubscriptionCommand request, CancellationToken cancellationToken)
    {
        var userLabels = await _userLabelRepository
            .GetAll()
            .Where(ul => ul.UserId == null || ul.UserId == _user.Id)
            .ToListAsync(cancellationToken);

        var matchingLabelIds = userLabels
            .Where(ul => request.Labels.Contains(ul.Id))
            .Select(ul => ul.Id)
            .ToList();

        var subscription = new Subscription
        {
            UserId = _user.Id,
            Name = request.Name,
            Amount = request.Amount,
            CurrencyId = request.CurrencyId,
            StartDate = request.StartDate,
            Frequency = request.Frequency,
            Interval = request.Interval,
            NextOccurrence = request.StartDate,
            SubscriptionLabels = matchingLabelIds.Select(id => new SubscriptionLabel
            {
                UserLabelId = id
            }).ToList()
        };

        await _subscriptionRepository.AddAsync(subscription);
        await _subscriptionRepository.SaveChangesAsync();

        return BaseResponse.Response(new { }, HttpStatusCode.OK);
    }
}
