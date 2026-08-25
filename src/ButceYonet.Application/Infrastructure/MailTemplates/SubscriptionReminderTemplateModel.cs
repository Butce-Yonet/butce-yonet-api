namespace ButceYonet.Application.Infrastructure.MailTemplates;

public class SubscriptionReminderTemplateModel
{
    public string UserName { get; set; }
    public List<SubscriptionReminderItem> OverdueItems { get; set; } = new();
    public List<SubscriptionReminderItem> UpcomingItems { get; set; } = new();
    public int Year { get; set; }
}

public class SubscriptionReminderItem
{
    public string Name { get; set; }
    public decimal? Amount { get; set; }
    public string CurrencySymbol { get; set; }
    public bool IsSymbolRight { get; set; }
    public DateTime DueDate { get; set; }

    /// <summary>Negatifse kaç gün gecikti, pozitifse kaç gün sonra vadesi geliyor</summary>
    public int DaysDiff { get; set; }
}
