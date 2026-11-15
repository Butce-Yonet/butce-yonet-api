using DotBoil.FCM;
using Microsoft.Extensions.Logging;

namespace ButceYonet.Application.Infrastructure.Notifications;

public class PushNotificationSender : IPushNotificationSender
{
    private readonly IFCMService _fcmService;
    private readonly INotificationLocalizer _localizer;
    private readonly ILogger<PushNotificationSender> _logger;

    public PushNotificationSender(
        IFCMService fcmService,
        INotificationLocalizer localizer,
        ILogger<PushNotificationSender> logger)
    {
        _fcmService = fcmService;
        _localizer = localizer;
        _logger = logger;
    }

    public async Task<FCMSendResult?> SendAsync(
        NotificationRecipient recipient,
        string titleKey,
        string bodyKey,
        object[] args,
        IReadOnlyDictionary<string, string>? data = null,
        CancellationToken cancellationToken = default)
    {
        if (recipient is null)
            return null;

        if (!recipient.IsChannelEnabled(NotificationChannels.MobileNotification))
        {
            _logger.LogDebug("Push {Key} skipped for user {UserId}: MobileNotification channel disabled.", bodyKey, recipient.UserId);
            return null;
        }

        var title = await _localizer.FormatAsync(recipient.Language, NotificationLocalizer.NotificationGroup, titleKey, args);
        var body = await _localizer.FormatAsync(recipient.Language, NotificationLocalizer.NotificationGroup, bodyKey, args);

        // FCM token'ları AuthGuard kullanıcı Id'si (nameidentifier claim) ile kaydediliyor.
        return await _fcmService.SendToUser(recipient.UserId.ToString(), new FCMNotification
        {
            Title = title,
            Body = body,
            Data = data
        }, cancellationToken);
    }
}
