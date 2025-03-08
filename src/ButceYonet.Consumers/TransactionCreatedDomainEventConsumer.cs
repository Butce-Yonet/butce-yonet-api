using ButceYonet.Application.Domain.Events;
using DotBoil.MassTransit.Attributes;
using DotBoil.MassTransit.Consumers;
using MassTransit;

namespace ButceYonet.Consumers;

[Consumer("transaction-created")]
public class TransactionCreatedDomainEventConsumer : BaseConsumer<TransactionCreatedDomainEvent>
{
    public TransactionCreatedDomainEventConsumer(IServiceProvider serviceProvider) : base(serviceProvider)
    {
    }

    public override Task ConsumeEvent(ConsumeContext<TransactionCreatedDomainEvent> context)
    {
        return Task.CompletedTask;
    }
}