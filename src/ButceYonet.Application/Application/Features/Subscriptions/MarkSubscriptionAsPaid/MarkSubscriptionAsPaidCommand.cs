using DotBoil.Entities;
using MediatR;

namespace ButceYonet.Application.Application.Features.Subscriptions.MarkSubscriptionAsPaid;

public class MarkSubscriptionAsPaidCommand : IRequest<BaseResponse>
{
    public int SubscriptionId { get; set; }
    public decimal Amount { get; set; }
    public int CurrencyId { get; set; }
    public DateTime? PaidDate { get; set; }
}
