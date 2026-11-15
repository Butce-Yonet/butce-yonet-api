using DotBoil.FCM;

namespace ButceYonet.Application.Infrastructure.Notifications;

public interface IPushNotificationSender
{
    /// <summary>
    /// "{titleKey}" ve "{bodyKey}" metinlerini alıcının dilinde hazırlayıp kullanıcının tüm cihazlarına FCM ile gönderir.
    /// MobileNotification kanalı kapalıysa (ya da tercih satırı yoksa) göndermez ve null döner.
    /// </summary>
    Task<FCMSendResult?> SendAsync(
        NotificationRecipient recipient,
        string titleKey,
        string bodyKey,
        object[] args,
        IReadOnlyDictionary<string, string>? data = null,
        CancellationToken cancellationToken = default);
}
