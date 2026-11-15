using System.Globalization;
using ButceYonet.Application.Infrastructure.MailTemplates;
using DotBoil;
using DotBoil.Configuration;
using DotBoil.Email;
using DotBoil.Email.Configuration;
using DotBoil.Email.Models;
using DotBoil.TemplateEngine;
using Microsoft.Extensions.Logging;

namespace ButceYonet.Application.Infrastructure.Notifications;

public class MailNotificationSender : IMailNotificationSender
{
    private readonly IRazorRenderer _razorRenderer;
    private readonly IMailSender _mailSender;
    private readonly INotificationLocalizer _localizer;
    private readonly ILogger<MailNotificationSender> _logger;

    public MailNotificationSender(
        IRazorRenderer razorRenderer,
        IMailSender mailSender,
        INotificationLocalizer localizer,
        ILogger<MailNotificationSender> logger)
    {
        _razorRenderer = razorRenderer;
        _mailSender = mailSender;
        _localizer = localizer;
        _logger = logger;
    }

    public async Task<bool> SendAsync<TModel>(
        NotificationRecipient recipient,
        string templateName,
        string subjectKey,
        TModel model,
        bool ignorePreferences = false,
        CancellationToken cancellationToken = default)
        where TModel : LocalizedMailTemplateModel
    {
        if (recipient is null || string.IsNullOrWhiteSpace(recipient.Email))
            return false;

        if (!ignorePreferences && !recipient.IsChannelEnabled(NotificationChannels.Email))
        {
            _logger.LogDebug("Mail {Template} skipped for user {UserId}: Email channel disabled.", templateName, recipient.UserId);
            return false;
        }

        model.Bind(_localizer, recipient.Language);

        // Şablon içindeki ToString("N2") / tarih formatları kullanıcının kültürüyle çalışsın.
        var previousCulture = CultureInfo.CurrentCulture;
        var previousUiCulture = CultureInfo.CurrentUICulture;
        string body;
        try
        {
            CultureInfo.CurrentCulture = recipient.Culture;
            CultureInfo.CurrentUICulture = recipient.Culture;
            body = await _razorRenderer.RenderAsync(templateName, model);
        }
        finally
        {
            CultureInfo.CurrentCulture = previousCulture;
            CultureInfo.CurrentUICulture = previousUiCulture;
        }

        var subject = await _localizer.GetAsync(recipient.Language, NotificationLocalizer.MailGroup, subjectKey);

        var serverSetting = DotBoilApp.Configuration.GetConfigurations<EmailOptions>().ServerSettings.FirstOrDefault();

        await _mailSender.SendAsync(serverSetting.Value, new Message
        {
            From = new List<string> { serverSetting.Value.EmailAddress },
            To = new List<string> { recipient.Email },
            Attachments = new List<Attachment>(),
            Body = body,
            Subject = subject
        });

        return true;
    }
}
