using System.Globalization;

namespace ButceYonet.Application.Infrastructure.Notifications;

public sealed class NotificationRecipient
{
    public int UserId { get; init; }
    public string Email { get; init; }
    public string DisplayName { get; init; }

    /// <summary>Normalize edilmiş dil kodu ("TR" / "EN").</summary>
    public string Language { get; init; } = NotificationLanguage.Default;

    public CultureInfo Culture => NotificationLanguage.GetCulture(Language);

    /// <summary>
    /// Kullanıcının açık olarak işaretlediği kanallar. AuthGuard'daki davranışla aynı şekilde
    /// tercih satırı olmayan kanal kapalı kabul edilir.
    /// </summary>
    public IReadOnlySet<string> EnabledChannels { get; init; } = new HashSet<string>();

    public bool IsChannelEnabled(string channel) => EnabledChannels.Contains(channel);
}
