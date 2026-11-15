namespace ButceYonet.Application.Infrastructure.Notifications;

public interface INotificationRecipientProvider
{
    /// <summary>Pasif (IsPassive) kullanıcı için null döner.</summary>
    Task<NotificationRecipient?> GetAsync(int userId, CancellationToken cancellationToken = default);

    /// <summary>
    /// İşlemsel mailler (şifre sıfırlama, hoş geldin) içindir ve pasif kullanıcıyı da döner:
    /// pasif hesaba kod gönderip göndermemeye AuthGuard karar verir, biz sadece iletiriz.
    /// </summary>
    Task<NotificationRecipient?> GetByEmailAsync(string email, CancellationToken cancellationToken = default);

    /// <summary>Toplu bildirimler (job'lar, sinyaller) içindir; pasif kullanıcılar sonuçta yer almaz.</summary>
    Task<IReadOnlyDictionary<int, NotificationRecipient>> GetManyAsync(IEnumerable<int> userIds, CancellationToken cancellationToken = default);
}
