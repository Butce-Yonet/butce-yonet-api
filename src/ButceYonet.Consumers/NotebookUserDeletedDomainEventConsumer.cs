using ButceYonet.Application.Domain.Events;
using DotBoil.MassTransit.Attributes;
using DotBoil.MassTransit.Consumers;
using MassTransit;

namespace ButceYonet.Consumers;

[Consumer("notebook-user-deleted")]
public class NotebookUserDeletedDomainEventConsumer : BaseConsumer<NotebookUserDeletedDomainEvent>
{
    public NotebookUserDeletedDomainEventConsumer(IServiceProvider serviceProvider) : base(serviceProvider)
    {
    }

    public override Task ConsumeEvent(ConsumeContext<NotebookUserDeletedDomainEvent> context)
    {
        return Task.CompletedTask;
    }
}