namespace ButceYonet.Application.Infrastructure.Notifications;

public interface INotificationLocalizer
{
    /// <summary>
    /// Metni istenen dilde getirir. Bulunamazsa varsayılan dile (TR), o da yoksa anahtarın kendisine düşer;
    /// eksik bir çeviri yüzünden gönderim patlamaz, sadece uyarı loglanır.
    /// </summary>
    Task<string> GetAsync(string language, string group, string key);

    /// <summary>GetAsync + string.Format({0}, {1}...) — formatlama kullanıcının kültürüyle yapılır.</summary>
    Task<string> FormatAsync(string language, string group, string key, params object[] args);
}
