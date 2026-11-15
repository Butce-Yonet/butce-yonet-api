using System.Globalization;
using System.Text.Encodings.Web;
using ButceYonet.Application.Infrastructure.Notifications;
using Microsoft.AspNetCore.Html;

namespace ButceYonet.Application.Infrastructure.MailTemplates;

// Şablonlar metinleri @await Model.Text("Anahtar", arg0, arg1...) ile çeker.
// Çeviri değerleri Localizations tablosundan gelir ve güvenilir kabul edilir (<strong> gibi HTML içerebilir);
// argümanlar ise kullanıcı verisi olabileceği için HTML-encode edilir.
public abstract class LocalizedMailTemplateModel
{
    private INotificationLocalizer _localizer;

    public string Language { get; private set; } = NotificationLanguage.Default;
    public CultureInfo Culture { get; private set; } = NotificationLanguage.GetCulture(NotificationLanguage.Default);
    public int Year { get; set; } = DateTime.UtcNow.Year;

    internal void Bind(INotificationLocalizer localizer, string language)
    {
        _localizer = localizer;
        Language = NotificationLanguage.Normalize(language);
        Culture = NotificationLanguage.GetCulture(Language);
    }

    public async Task<IHtmlContent> Text(string key, params object[] args)
    {
        if (_localizer is null)
            return new HtmlString(HtmlEncoder.Default.Encode(key));

        var encodedArgs = (args ?? Array.Empty<object>())
            .Select(a => (object)HtmlEncoder.Default.Encode(FormatArg(a)))
            .ToArray();

        var text = await _localizer.FormatAsync(Language, NotificationLocalizer.MailGroup, key, encodedArgs);
        return new HtmlString(text);
    }

    public string FormatAmount(decimal amount, string currencySymbol, bool isSymbolRight)
    {
        var formatted = amount.ToString("N2", Culture);
        return isSymbolRight ? $"{formatted} {currencySymbol}" : $"{currencySymbol} {formatted}";
    }

    public string FormatDate(DateTime date) => date.ToString("d", Culture);

    private string FormatArg(object arg) => arg switch
    {
        null => string.Empty,
        DateTime date => FormatDate(date),
        IFormattable formattable => formattable.ToString(null, Culture),
        _ => arg.ToString() ?? string.Empty
    };
}
