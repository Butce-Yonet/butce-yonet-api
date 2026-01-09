using DotBoil.AuthGuard.Application.Domain.DomainEvents;
using DotBoil.MassTransit.Attributes;
using DotBoil.MassTransit.Consumers;
using MassTransit;

namespace ButceYonet.Consumers;

[Consumer("forgot-password")]
public class ForgotPasswordOptCodeCreatedDomainEventConsumer : BaseConsumer<ForgotPasswordOtpCodeCreatedDomainEvent>
{
    public ForgotPasswordOptCodeCreatedDomainEventConsumer(IServiceProvider serviceProvider) 
        : base(serviceProvider)
    {
    }

    public override Task ConsumeEvent(ConsumeContext<ForgotPasswordOtpCodeCreatedDomainEvent> context)
    {
        return Task.CompletedTask;
    }
}