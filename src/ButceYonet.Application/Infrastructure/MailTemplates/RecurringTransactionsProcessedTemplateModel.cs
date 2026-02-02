namespace ButceYonet.Application.Infrastructure.MailTemplates;

public class RecurringTransactionsProcessedTemplateModel
{
    public string UserName { get; set; }
    public DateTime ProcessedDate { get; set; }
    public List<ProcessedRecurringItem> Items { get; set; } = new();
    public int Year { get; set; }
}

public class ProcessedRecurringItem
{
    public string Name { get; set; }
    public string Description { get; set; }
    public string TransactionTypeDisplay { get; set; }
    public bool IsIncome { get; set; }
    public decimal Amount { get; set; }
    public string CurrencySymbol { get; set; }
    public bool IsSymbolRight { get; set; }
}
