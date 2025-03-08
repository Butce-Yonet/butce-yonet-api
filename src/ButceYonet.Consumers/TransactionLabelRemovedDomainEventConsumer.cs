using ButceYonet.Application.Domain.Events;
using DotBoil.MassTransit.Attributes;
using DotBoil.MassTransit.Consumers;
using MassTransit;

namespace ButceYonet.Consumers;

[Consumer("transaction-label-removed")]
public class TransactionLabelRemovedDomainEventConsumer : BaseConsumer<TransactionLabelRemovedDomainEvent>
{
    public TransactionLabelRemovedDomainEventConsumer(IServiceProvider serviceProvider) : base(serviceProvider)
    {
    }

    public override Task ConsumeEvent(ConsumeContext<TransactionLabelRemovedDomainEvent> context)
    {
        return Task.CompletedTask;
    }
}