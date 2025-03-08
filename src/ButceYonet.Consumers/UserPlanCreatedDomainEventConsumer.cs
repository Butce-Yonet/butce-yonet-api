using ButceYonet.Application.Domain.Events;
using DotBoil.MassTransit.Attributes;
using DotBoil.MassTransit.Consumers;
using MassTransit;

namespace ButceYonet.Consumers;

[Consumer("user-plan-created")]
public class UserPlanCreatedDomainEventConsumer : BaseConsumer<UserPlanCreatedDomainEvent> 
{
    public UserPlanCreatedDomainEventConsumer(IServiceProvider serviceProvider) : base(serviceProvider)
    {
    }

    public override Task ConsumeEvent(ConsumeContext<UserPlanCreatedDomainEvent> context)
    {
        return Task.CompletedTask;
    }
}