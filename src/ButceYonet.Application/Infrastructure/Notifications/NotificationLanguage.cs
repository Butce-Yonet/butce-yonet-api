using System.Globalization;

namespace ButceYonet.Application.Infrastructure.Notifications;

// User.ActiveLanguage "tr", "en", "en-US" gibi gelebilir; Localizations tablosu ise "TR"/"EN" kullanıyor.
// Bilinmeyen ya da boş dil TR'ye düşer.
public static class NotificationLanguage
{
    public const string Turkish = "TR";
    public const string English = "EN";
    public const string Default = Turkish;

    private static readonly CultureInfo TurkishCulture = CultureInfo.GetCultureInfo("tr-TR");
    private static readonly CultureInfo EnglishCulture = CultureInfo.GetCultureInfo("en-US");

    public static string Normalize(string? language)
    {
        if (string.IsNullOrWhiteSpace(language))
            return Default;

        var code = language.Trim().Split('-', '_')[0].ToUpperInvariant();

        return code switch
        {
            Turkish => Turkish,
            English => English,
            _ => Default
        };
    }

    public static CultureInfo GetCulture(string language)
    {
        return Normalize(language) == English ? EnglishCulture : TurkishCulture;
    }
}
