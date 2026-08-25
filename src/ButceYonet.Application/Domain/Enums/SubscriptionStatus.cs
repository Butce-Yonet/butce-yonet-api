namespace ButceYonet.Application.Domain.Enums;

public enum SubscriptionStatus
{
    /// <summary>
    /// Vadesi yaklaşıyor
    /// </summary>
    Upcoming,

    /// <summary>
    /// Yakın zamanda ödendi olarak işaretlendi
    /// </summary>
    Paid,

    /// <summary>
    /// Vadesi geçti
    /// </summary>
    Overdue
}
