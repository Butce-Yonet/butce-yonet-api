namespace ButceYonet.Application.Infrastructure.MailTemplates;

public class RecurringTransactionEndedTemplateModel
{
    public string UserName { get; set; }
    public DateTime EndedDate { get; set; }
    public List<EndedRecurringItem> EndedIncomes { get; set; } = new();
    public List<EndedRecurringItem> EndedExpenses { get; set; } = new();
    public int Year { get; set; }
}

public class EndedRecurringItem
{
    public string Name { get; set; }
    public string Description { get; set; }
    public decimal Amount { get; set; }
    public string CurrencySymbol { get; set; }
    public bool IsSymbolRight { get; set; }
}
