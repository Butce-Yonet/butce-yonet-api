namespace ButceYonet.Application.Domain.Enums;

public enum TransactionTypes
{
    /// <summary>
    /// Gelir
    /// </summary>
    Income, 
    
    /// <summary>
    /// Gider
    /// </summary>
    Expense,

    /// <summary>
    /// Birikim (Hedef) — para çıkmaz, yer değiştirir. Gider olarak sayılmaz.
    /// Yalnızca hedefe katkı akışıyla oluşturulur, genel Transaction CRUD'dan girilemez.
    /// </summary>
    Saving
}