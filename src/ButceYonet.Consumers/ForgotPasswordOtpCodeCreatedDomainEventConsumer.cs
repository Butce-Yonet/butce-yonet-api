using ButceYonet.Application.Infrastructure.MailTemplates;
using DotBoil;
using DotBoil.AuthGuard.Application.Domain.DomainEvents;
using DotBoil.Configuration;
using DotBoil.Email;
using DotBoil.Email.Configuration;
using DotBoil.Email.Models;
using DotBoil.MassTransit.Attributes;
using DotBoil.MassTransit.Consumers;
using DotBoil.TemplateEngine;
using MassTransit;

namespace ButceYonet.Consumers;

[Consumer("forgot-password")]
public class ForgotPasswordOtpCodeCreatedDomainEventConsumer : BaseConsumer<ForgotPasswordOtpCodeCreatedDomainEvent>
{
    private readonly IServiceProvider _serviceProvider;
    private IRazorRenderer _razorRenderer;
    private IMailSender _mailSender;
    
    public ForgotPasswordOtpCodeCreatedDomainEventConsumer(IServiceProvider serviceProvider) 
        : base(serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public override async Task ConsumeEvent(ConsumeContext<ForgotPasswordOtpCodeCreatedDomainEvent> context)
    {
        InitializeDependencies();
        
        var forgotPasswordOtpCodeCreatedTemplateModel = new ForgotPasswordOtpCodeCreatedTemplateModel
        {
            Email = context.Message.Email,
            OtpCode = context.Message.OtpCode,
            Year = DateTime.Now.Year
        };

        var mailContent = await _razorRenderer.RenderAsync("ForgotPasswordOtpCodeCreatedTemplate",
            forgotPasswordOtpCodeCreatedTemplateModel);
        var mailConfiguration = DotBoilApp.Configuration.GetConfigurations<EmailOptions>();
        var serverSettings = mailConfiguration.ServerSettings;
        var serverSetting = serverSettings.FirstOrDefault();
        
        await _mailSender.SendAsync(serverSetting.Value, new Message()
        {
            From = new List<string>() { serverSetting.Value.EmailAddress },
            To = new List<string>() { context.Message.Email },
            Attachments = new List<Attachment>(),
            Body = mailContent,
            Subject = "Bütçe Yönet"
        });
    }

    private void InitializeDependencies()
    {
        using var scope = _serviceProvider.CreateScope();
        _razorRenderer = scope.ServiceProvider.GetService<IRazorRenderer>();
        _mailSender = scope.ServiceProvider.GetService<IMailSender>();
    }
}