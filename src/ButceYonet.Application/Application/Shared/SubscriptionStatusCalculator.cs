using ButceYonet.Application.Domain.Enums;

namespace ButceYonet.Application.Application.Shared;

/// <summary>
/// Subscription durumu (Ödendi/Gecikmiş/Yaklaşan) NextOccurrence ve LastPaidDate'ten
/// türetilir, DB'de ayrı bir kolon olarak tutulmaz.
/// </summary>
public static class SubscriptionStatusCalculator
{
    /// <summary>LastPaidDate bu kadar gün içindeyse "Ödendi" sayılır</summary>
    public const int PaidWindowDays = 3;

    /// <summary>NextOccurrence bugünden bu kadar gün sonrasına kadarsa "Yaklaşan" sayılır</summary>
    public const int UpcomingWindowDays = 7;

    public static SubscriptionStatus? Calculate(DateTime? nextOccurrence, DateTime? lastPaidDate, DateTime today)
    {
        if (lastPaidDate.HasValue && lastPaidDate.Value.Date >= today.AddDays(-PaidWindowDays))
            return SubscriptionStatus.Paid;

        if (nextOccurrence.HasValue && nextOccurrence.Value.Date < today)
            return SubscriptionStatus.Overdue;

        if (nextOccurrence.HasValue &&
            nextOccurrence.Value.Date >= today &&
            nextOccurrence.Value.Date <= today.AddDays(UpcomingWindowDays))
            return SubscriptionStatus.Upcoming;

        return null;
    }
}
