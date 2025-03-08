using ButceYonet.Application.Domain.Events;
using DotBoil.MassTransit.Attributes;
using DotBoil.MassTransit.Consumers;
using MassTransit;

namespace ButceYonet.Consumers;

[Consumer("transaction-label-added")]
public class TransactionLabelAddedDomainEventConsumer : BaseConsumer<TransactionLabelAddedDomainEvent>
{
    public TransactionLabelAddedDomainEventConsumer(IServiceProvider serviceProvider) : base(serviceProvider)
    {
    }

    public override Task ConsumeEvent(ConsumeContext<TransactionLabelAddedDomainEvent> context)
    {
        return Task.CompletedTask;
    }
}