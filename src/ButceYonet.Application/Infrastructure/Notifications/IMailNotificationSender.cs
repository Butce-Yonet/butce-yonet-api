using ButceYonet.Application.Infrastructure.MailTemplates;

namespace ButceYonet.Application.Infrastructure.Notifications;

public interface IMailNotificationSender
{
    /// <summary>
    /// Şablonu alıcının dilinde render edip gönderir.
    /// Kullanıcının Email kanalı kapalıysa (ya da tercih satırı yoksa) göndermez ve false döner.
    /// </summary>
    /// <param name="ignorePreferences">
    /// Şifre sıfırlama, hoş geldin gibi işlemsel mailler için true: kullanıcı tercihinden bağımsız gider.
    /// </param>
    Task<bool> SendAsync<TModel>(
        NotificationRecipient recipient,
        string templateName,
        string subjectKey,
        TModel model,
        bool ignorePreferences = false,
        CancellationToken cancellationToken = default)
        where TModel : LocalizedMailTemplateModel;
}
