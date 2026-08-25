using System.Net;
using AutoMapper;
using ButceYonet.Application.Application.Interfaces;
using ButceYonet.Application.Application.Shared;
using ButceYonet.Application.Application.Shared.Dtos;
using ButceYonet.Application.Domain.Entities;
using ButceYonet.Application.Domain.Enums;
using ButceYonet.Application.Infrastructure.Data;
using DotBoil.Caching;
using DotBoil.EFCore;
using DotBoil.EFCore.Extensions;
using DotBoil.Entities;
using DotBoil.Localization;
using DotBoil.Parameter;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace ButceYonet.Application.Application.Features.Subscriptions.GetSubscriptions;

public class GetSubscriptionsQueryHandler : BaseHandler<GetSubscriptionsQuery, BaseResponse>
{
    private readonly IRepository<Subscription, ButceYonetDbContext> _subscriptionRepository;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public GetSubscriptionsQueryHandler(
        ICache cache,
        IUser user,
        IMapper mapper,
        ILocalize localize,
        IParameterManager parameter,
        IUserPlanValidator userPlanValidator,
        IRepository<Subscription, ButceYonetDbContext> subscriptionRepository,
        IHttpContextAccessor httpContextAccessor)
        : base(cache, user, mapper, localize, parameter, userPlanValidator)
    {
        _subscriptionRepository = subscriptionRepository;
        _httpContextAccessor = httpContextAccessor;
    }

    public override async Task<BaseResponse> ExecuteRequest(GetSubscriptionsQuery request, CancellationToken cancellationToken)
    {
        var paginationRequest = new PaginationFilter(
            int.Parse(_httpContextAccessor.HttpContext.Request.Query["PageNumber"].ToString()),
            int.Parse(_httpContextAccessor.HttpContext.Request.Query["PageSize"].ToString()));

        var today = DateTime.UtcNow.Date;
        var paidWindowStart = today.AddDays(-SubscriptionStatusCalculator.PaidWindowDays);
        var upcomingWindowEnd = today.AddDays(SubscriptionStatusCalculator.UpcomingWindowDays);

        var query = _subscriptionRepository
            .GetAll()
            .Where(s => s.UserId == _user.Id);

        if (request.Status.HasValue)
        {
            switch (request.Status.Value)
            {
                case SubscriptionStatus.Paid:
                    query = query.Where(s => s.LastPaidDate.HasValue && s.LastPaidDate.Value.Date >= paidWindowStart);
                    break;
                case SubscriptionStatus.Overdue:
                    query = query.Where(s =>
                        !(s.LastPaidDate.HasValue && s.LastPaidDate.Value.Date >= paidWindowStart) &&
                        s.NextOccurrence.HasValue && s.NextOccurrence.Value.Date < today);
                    break;
                case SubscriptionStatus.Upcoming:
                    query = query.Where(s =>
                        !(s.LastPaidDate.HasValue && s.LastPaidDate.Value.Date >= paidWindowStart) &&
                        s.NextOccurrence.HasValue && s.NextOccurrence.Value.Date >= today && s.NextOccurrence.Value.Date <= upcomingWindowEnd);
                    break;
            }
        }

        var subscriptions = await query
            .Include(s => s.Currency)
            .Include(s => s.SubscriptionLabels.Where(sl => !sl.IsDeleted))
            .ThenInclude(sl => sl.UserLabel)
            .OrderBy(s => s.NextOccurrence ?? DateTime.MaxValue)
            .PaginateAsync(paginationRequest);

        var subscriptionDtos = _mapper.Map<List<SubscriptionDto>>(subscriptions.Items);
        foreach (var (entity, dto) in subscriptions.Items.Zip(subscriptionDtos))
            dto.Status = SubscriptionStatusCalculator.Calculate(entity.NextOccurrence, entity.LastPaidDate, today);

        var paginatedResponse = new PaginatedModel<SubscriptionDto>(
            subscriptions.PageNumber,
            subscriptions.PageSize,
            subscriptions.TotalPages,
            subscriptions.TotalRecords,
            subscriptionDtos);

        return BaseResponse.Response(paginatedResponse, HttpStatusCode.OK);
    }
}
