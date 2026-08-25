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

namespace ButceYonet.Application.Application.Features.Subscriptions.UpdateSubscription;

public class UpdateSubscriptionCommandHandler : BaseHandler<UpdateSubscriptionCommand, BaseResponse>
{
    private readonly IRepository<Subscription, ButceYonetDbContext> _subscriptionRepository;
    private readonly IRepository<SubscriptionLabel, ButceYonetDbContext> _subscriptionLabelRepository;
    private readonly IRepository<UserLabel, ButceYonetDbContext> _userLabelRepository;
    private readonly IRecurringTransactionIntervalsService _recurringTransactionIntervalsService;

    public UpdateSubscriptionCommandHandler(
        ICache cache,
        IUser user,
        IMapper mapper,
        ILocalize localize,
        IParameterManager parameter,
        IUserPlanValidator userPlanValidator,
        IRepository<Subscription, ButceYonetDbContext> subscriptionRepository,
        IRepository<SubscriptionLabel, ButceYonetDbContext> subscriptionLabelRepository,
        IRepository<UserLabel, ButceYonetDbContext> userLabelRepository,
        IRecurringTransactionIntervalsService recurringTransactionIntervalsService)
        : base(cache, user, mapper, localize, parameter, userPlanValidator)
    {
        _subscriptionRepository = subscriptionRepository;
        _subscriptionLabelRepository = subscriptionLabelRepository;
        _userLabelRepository = userLabelRepository;
        _recurringTransactionIntervalsService = recurringTransactionIntervalsService;
    }

    public override async Task<BaseResponse> ExecuteRequest(UpdateSubscriptionCommand request, CancellationToken cancellationToken)
    {
        var subscription = await _subscriptionRepository
            .Get()
            .Where(s => s.Id == request.Id && s.UserId == _user.Id)
            .Include(s => s.SubscriptionLabels.Where(sl => !sl.IsDeleted))
            .FirstOrDefaultAsync(cancellationToken);

        if (subscription is null)
            throw new NotFoundException(typeof(Subscription));

        var originalStartDate = subscription.StartDate;
        var originalFrequency = subscription.Frequency;
        var originalInterval = subscription.Interval;

        subscription.Name = request.Name;
        subscription.Amount = request.Amount;
        subscription.CurrencyId = request.CurrencyId;
        subscription.StartDate = request.StartDate;
        subscription.Frequency = request.Frequency;
        subscription.Interval = request.Interval;

        var scheduleChanged = originalStartDate != request.StartDate
            || originalFrequency != request.Frequency
            || originalInterval != request.Interval;
        if (scheduleChanged)
            subscription.NextOccurrence = _recurringTransactionIntervalsService.CalculateInterval(request.StartDate, request.Frequency, request.Interval);

        foreach (var label in subscription.SubscriptionLabels)
        {
            label.IsDeleted = true;
            _subscriptionLabelRepository.Update(label);
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
            await _subscriptionLabelRepository.AddAsync(new SubscriptionLabel
            {
                SubscriptionId = subscription.Id,
                UserLabelId = labelId
            });
        }

        _subscriptionRepository.Update(subscription);
        await _subscriptionRepository.SaveChangesAsync();

        return BaseResponse.Response(new { }, HttpStatusCode.OK);
    }
}
