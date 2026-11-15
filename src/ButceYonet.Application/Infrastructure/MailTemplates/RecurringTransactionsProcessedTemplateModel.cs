namespace ButceYonet.Application.Infrastructure.MailTemplates;

public class RecurringTransactionsProcessedTemplateModel : LocalizedMailTemplateModel
{
    public string UserName { get; set; }
    public DateTime ProcessedDate { get; set; }
    public List<ProcessedRecurringItem> Items { get; set; } = new();
}

public class ProcessedRecurringItem
{
    public string Name { get; set; }
    public string Description { get; set; }
    public bool IsIncome { get; set; }
    public decimal Amount { get; set; }
    public string CurrencySymbol { get; set; }
    public bool IsSymbolRight { get; set; }
}
