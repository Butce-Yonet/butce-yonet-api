using ButceYonet.Application.Domain.Events;
using DotBoil.MassTransit.Attributes;
using DotBoil.MassTransit.Consumers;
using MassTransit;

namespace ButceYonet.Consumers;

[Consumer("transaction-updated")]
public class TransactionUpdatedDomainEventConsumer : BaseConsumer<TransactionUpdatedDomainEvent>
{
    public TransactionUpdatedDomainEventConsumer(IServiceProvider serviceProvider) : base(serviceProvider)
    {
    }

    public override Task ConsumeEvent(ConsumeContext<TransactionUpdatedDomainEvent> context)
    {
        return Task.CompletedTask;
    }
}