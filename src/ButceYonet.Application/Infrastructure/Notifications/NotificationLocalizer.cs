using DotBoil.Localization;
using Microsoft.Extensions.Logging;

namespace ButceYonet.Application.Infrastructure.Notifications;

public class NotificationLocalizer : INotificationLocalizer
{
    public const string MailGroup = "ButceYonet.Mail";
    public const string NotificationGroup = "ButceYonet.Notification";

    private readonly ILocalize _localize;
    private readonly ILogger<NotificationLocalizer> _logger;

    public NotificationLocalizer(ILocalize localize, ILogger<NotificationLocalizer> logger)
    {
        _localize = localize;
        _logger = logger;
    }

    public async Task<string> GetAsync(string language, string group, string key)
    {
        language = NotificationLanguage.Normalize(language);

        var text = await TryLocalizeAsync(language, group, key);
        if (text is not null)
            return text;

        if (language != NotificationLanguage.Default)
        {
            text = await TryLocalizeAsync(NotificationLanguage.Default, group, key);
            if (text is not null)
            {
                _logger.LogWarning("Localization {Group}:{Key} is missing for {Language}, fell back to {Default}.",
                    group, key, language, NotificationLanguage.Default);
                return text;
            }
        }

        _logger.LogWarning("Localization {Group}:{Key} is missing for {Language}.", group, key, language);
        return key;
    }

    public async Task<string> FormatAsync(string language, string group, string key, params object[] args)
    {
        var text = await GetAsync(language, group, key);
        if (args is null || args.Length == 0)
            return text;

        try
        {
            return string.Format(NotificationLanguage.GetCulture(language), text, args);
        }
        catch (FormatException ex)
        {
            _logger.LogWarning(ex, "Localization {Group}:{Key} ({Language}) has an invalid format string.", group, key, language);
            return text;
        }
    }

    private async Task<string?> TryLocalizeAsync(string language, string group, string key)
    {
        try
        {
            return await _localize.LocalizeTextWithLanguage(language, group, key);
        }
        catch (Exception)
        {
            // DotBoil.Localization eksik anahtarda (ve Redis/DB hatasında) LocalizeException fırlatır.
            return null;
        }
    }
}
