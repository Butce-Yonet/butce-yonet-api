using ButceYonet.Application.Domain.Enums;

namespace ButceYonet.Application.Application.Shared.Dtos;

public class SubscriptionDto
{
    public int Id { get; set; }
    public string Name { get; set; }
    public decimal? Amount { get; set; }
    public CurrencyDto Currency { get; set; }
    public DateTime StartDate { get; set; }
    public RecurringTransactionIntervals Frequency { get; set; }
    public int? Interval { get; set; }
    public DateTime? NextOccurrence { get; set; }
    public DateTime? LastPaidDate { get; set; }
    public decimal? LastPaidAmount { get; set; }
    public SubscriptionStatus? Status { get; set; }
    public List<UserLabelDto> Labels { get; set; } = new();
}
