namespace ButceYonet.Application.Application.Shared.Dtos;

/// <summary>
/// Para birimi bazında aktif dönem bütçe özeti (dashboard kartı).
/// </summary>
public class BudgetSummaryDto
{
    public CurrencyDto Currency { get; set; }

    /// <summary>
    /// Genel bütçe varsa onun limiti/harcaması; yoksa etiket bütçelerinin toplamı.
    /// (Etiket toplamında çok etiketli bir işlem birden fazla bütçeye sayılabilir.)
    /// </summary>
    public decimal TotalAmount { get; set; }
    public decimal TotalSpent { get; set; }
    public decimal RemainingAmount { get; set; }
    public int BudgetCount { get; set; }
    public int OnTrackCount { get; set; }
    public int WarningCount { get; set; }
    public int ExceededCount { get; set; }
}
