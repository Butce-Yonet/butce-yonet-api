namespace ButceYonet.Application.Domain.Enums;

public enum RecurringTransactionIntervals
{
    /// <summary>
    /// Günlük
    /// </summary>
    Daily,
    
    /// <summary>
    /// Haftalık
    /// </summary>
    Weekly,
    
    /// <summary>
    /// Aylık
    /// </summary>
    Monthly,
    
    /// <summary>
    /// Yıllık
    /// </summary>
    Yearly,
    
    /// <summary>
    /// Ayın Son Günü
    /// </summary>
    LastDayOfTheMonth,
    
    /// <summary>
    /// Ayın İlk İş Günü
    /// </summary>
    FirstBusinessDayOfTheMonth,
    
    /// <summary>
    /// Ayın Son İş Günü
    /// </summary>
    LastBusinessDayOfTheMonth,
    
    /// <summary>
    /// Ayın X Günü
    /// </summary>
    XThOfTheMonth
}