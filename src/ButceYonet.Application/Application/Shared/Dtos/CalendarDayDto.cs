namespace ButceYonet.Application.Application.Shared.Dtos;

/// <summary>
/// Takvimdeki tek bir güne ait özet: toplam gelir/gider, işlem sayısı ve önizleme işlemleri.
/// </summary>
public class CalendarDayDto
{
    public DateTime Date { get; set; }

    /// <summary>Bu güne ait toplam gelir</summary>
    public decimal TotalIncome { get; set; }

    /// <summary>Bu güne ait toplam gider</summary>
    public decimal TotalExpense { get; set; }

    /// <summary>Bu güne ait net bakiye (Gelir - Gider)</summary>
    public decimal NetBalance { get; set; }

    /// <summary>Bu güne ait toplam işlem sayısı</summary>
    public int TransactionCount { get; set; }

    /// <summary>
    /// Takvim hücresinde gösterilecek ilk birkaç işlem. Tamamı için işlem listesi
    /// StartTime/EndTime filtresiyle ayrıca sorgulanmalıdır.
    /// </summary>
    public List<TransactionDto> PreviewTransactions { get; set; } = new();
}
