using ButceYonet.Application.Domain.Events;
using DotBoil.MassTransit.Attributes;
using DotBoil.MassTransit.Consumers;
using MassTransit;

namespace ButceYonet.Consumers;

[Consumer("transaction-deleted")]
public class TransactionDeletedDomainEventConsumer : BaseConsumer<TransactionDeletedDomainEvent>
{
    public TransactionDeletedDomainEventConsumer(IServiceProvider serviceProvider) : base(serviceProvider)
    {
    }

    public override Task ConsumeEvent(ConsumeContext<TransactionDeletedDomainEvent> context)
    {
        return Task.CompletedTask;
    }
}