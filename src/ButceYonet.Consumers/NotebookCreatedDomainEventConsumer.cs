using ButceYonet.Application.Domain.Events;
using DotBoil.MassTransit.Attributes;
using DotBoil.MassTransit.Consumers;
using MassTransit;

namespace ButceYonet.Consumers;

[Consumer("notebook-created")]
public class NotebookCreatedDomainEventConsumer : BaseConsumer<NotebookCreatedDomainEvent>
{
    public NotebookCreatedDomainEventConsumer(IServiceProvider serviceProvider) : base(serviceProvider)
    {
    }

    public override Task ConsumeEvent(ConsumeContext<NotebookCreatedDomainEvent> context)
    {
        return Task.CompletedTask;
    }
}