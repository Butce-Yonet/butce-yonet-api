using ButceYonet.Application.Infrastructure.MailTemplates;
using ButceYonet.Application.Infrastructure.Notifications;
using DotBoil.AuthGuard.Application.Domain.DomainEvents;
using DotBoil.MassTransit.Attributes;
using DotBoil.MassTransit.Consumers;
using MassTransit;

namespace ButceYonet.Consumers;

[Consumer("forgot-password")]
public class ForgotPasswordOtpCodeCreatedDomainEventConsumer : BaseConsumer<ForgotPasswordOtpCodeCreatedDomainEvent>
{
    private readonly IServiceProvider _serviceProvider;

    public ForgotPasswordOtpCodeCreatedDomainEventConsumer(IServiceProvider serviceProvider)
        : base(serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public override async Task ConsumeEvent(ConsumeContext<ForgotPasswordOtpCodeCreatedDomainEvent> context)
    {
        using var scope = _serviceProvider.CreateScope();
        var recipientProvider = scope.ServiceProvider.GetRequiredService<INotificationRecipientProvider>();
        var mailSender = scope.ServiceProvider.GetRequiredService<IMailNotificationSender>();

        // Kullanıcı bulunamazsa da kod gitmeli; dil varsayılana (TR) düşer.
        var recipient = await recipientProvider.GetByEmailAsync(context.Message.Email)
                        ?? new NotificationRecipient { Email = context.Message.Email };

        var model = new ForgotPasswordOtpCodeCreatedTemplateModel
        {
            Email = context.Message.Email,
            OtpCode = context.Message.OtpCode
        };

        // Şifre sıfırlama işlemsel bir mail: kullanıcının Email tercihinden bağımsız gönderilir.
        await mailSender.SendAsync(recipient, "ForgotPasswordOtpCodeCreatedTemplate", "ForgotPassword.Subject", model,
            ignorePreferences: true);
    }
}
