using DotBoil.Entities;

namespace ButceYonet.Application.Domain.Entities;

// AuthGuard'ın UserNotificationPreferences tablosunun salt okunur karşılığı.
// Tablo ve şeması AuthGuard migration'larıyla yönetilir; burada sadece okunur.
public class UserNotificationPreference : BaseEntity
{
    public int UserId { get; set; }
    public string Channel { get; set; }
    public bool IsEnabled { get; set; }
}
