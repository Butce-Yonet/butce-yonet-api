using ButceYonet.Application.Domain.Events;
using DotBoil.MassTransit.Attributes;
using DotBoil.MassTransit.Consumers;
using MassTransit;

namespace ButceYonet.Consumers;

[Consumer("notebook-user-created")]
public class NotebookUserCreatedDomainEventConsumer : BaseConsumer<NotebookUserCreatedDomainEvent>
{
    public NotebookUserCreatedDomainEventConsumer(IServiceProvider serviceProvider) : base(serviceProvider)
    {
    }

    public override Task ConsumeEvent(ConsumeContext<NotebookUserCreatedDomainEvent> context)
    {
        return Task.CompletedTask;
    }
}