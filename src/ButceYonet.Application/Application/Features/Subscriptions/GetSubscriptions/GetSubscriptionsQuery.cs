using ButceYonet.Application.Domain.Enums;
using DotBoil.Entities;
using MediatR;

namespace ButceYonet.Application.Application.Features.Subscriptions.GetSubscriptions;

public class GetSubscriptionsQuery : PaginationFilter, IRequest<BaseResponse>
{
    /// <summary>Belirtilmezse (Tümü) tüm abonelikler döner</summary>
    public SubscriptionStatus? Status { get; set; }
}
