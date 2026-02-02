namespace ButceYonet.Application.Application.Shared.Dtos;

/// <summary>
/// "En çok neye para harcıyorum" raporu için kategori bazlı harcama özeti.
/// </summary>
public class CategorySpendingReportDto
{
    public int NotebookId { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }

    /// <summary>
    /// Seçilen dönemde kategori bağımsız genel harcanan toplam tutar
    /// (NonCategorizedTransactionReport üzerinden hesaplanır).
    /// </summary>
    public decimal GeneralTotalAmount { get; set; }

    /// <summary>
    /// Kategori bazlı detaylar.
    /// </summary>
    public List<CategorySpendingReportItemDto> Items { get; set; } = new();
}

public class CategorySpendingReportItemDto
{
    /// <summary>Kategori adı (NotebookLabel.Name)</summary>
    public string CategoryName { get; set; }

    /// <summary>Seçilen dönemde bu kategori için toplam harcanan tutar (gider)</summary>
    public decimal Amount { get; set; }

    /// <summary>
    /// Toplam harcamadaki yüzdelik pay (0-100 arası).
    /// GeneralTotalAmount'a oranlanır.
    /// </summary>
    public decimal Percentage { get; set; }

    /// <summary>Önceki dönemde bu kategoride harcanan toplam tutar.</summary>
    public decimal PreviousAmount { get; set; }
}

