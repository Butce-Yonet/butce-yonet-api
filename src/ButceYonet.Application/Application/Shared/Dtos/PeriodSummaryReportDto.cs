namespace ButceYonet.Application.Application.Shared.Dtos;

/// <summary>
/// Dönem özeti raporu: seçilen dönem için toplam gelir, gider, net bakiye ve önceki dönemle kıyas.
/// </summary>
public class PeriodSummaryReportDto
{
    public int NotebookId { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }

    /// <summary>Seçilen dönem toplam gelir</summary>
    public decimal TotalIncome { get; set; }

    /// <summary>Seçilen dönem toplam gider</summary>
    public decimal TotalExpense { get; set; }

    /// <summary>Net bakiye (Gelir - Gider)</summary>
    public decimal NetBalance { get; set; }

    /// <summary>Önceki dönem başlangıç tarihi (karşılaştırma için)</summary>
    public DateTime? PreviousPeriodStartDate { get; set; }

    /// <summary>Önceki dönem bitiş tarihi</summary>
    public DateTime? PreviousPeriodEndDate { get; set; }

    /// <summary>Önceki dönem toplam gelir</summary>
    public decimal? PreviousTotalIncome { get; set; }

    /// <summary>Önceki dönem toplam gider</summary>
    public decimal? PreviousTotalExpense { get; set; }

    /// <summary>Önceki dönem net bakiye</summary>
    public decimal? PreviousNetBalance { get; set; }

    /// <summary>Gelirde önceki döneme göre yüzde değişim (artış/azalış). Null ise karşılaştırma yapılamadı.</summary>
    public decimal? IncomeChangePercent { get; set; }

    /// <summary>Giderde önceki döneme göre yüzde değişim. Null ise karşılaştırma yapılamadı.</summary>
    public decimal? ExpenseChangePercent { get; set; }

    /// <summary>Net bakiyede önceki döneme göre yüzde değişim. Null ise karşılaştırma yapılamadı.</summary>
    public decimal? NetBalanceChangePercent { get; set; }

    /// <summary>Para birimi filtresi uygulandıysa bilgisi</summary>
    public CurrencyDto Currency { get; set; }
}
