using ButceYonet.Application.Domain.Enums;

namespace ButceYonet.Application.Application.Shared.Dtos;

public class BudgetDto
{
    public int Id { get; set; }
    public decimal Amount { get; set; }
    public CurrencyDto Currency { get; set; }

    /// <summary>Genel (tüm giderler) bütçede null</summary>
    public UserLabelDto Label { get; set; }
    public bool IsGeneral { get; set; }
    public int PeriodStartDay { get; set; }

    // Aktif dönem durumu: DB'de tutulmaz, BudgetProgressCalculator ile hesaplanır.
    public DateTime PeriodStart { get; set; }
    public DateTime PeriodEnd { get; set; }
    public decimal SpentAmount { get; set; }
    public decimal RemainingAmount { get; set; }
    public decimal OverAmount { get; set; }

    /// <summary>Limit aşıldığında 100'ü geçebilir (örn. 130)</summary>
    public decimal Percentage { get; set; }
    public BudgetStatus Status { get; set; }
    public int DaysLeft { get; set; }

    /// <summary>Dönem sonuna kadar limitte kalmak için günlük harcanabilecek tutar</summary>
    public decimal DailyAllowance { get; set; }

    /// <summary>Sadece detayda dolu: aktif dönem dahil son dönemler, eskiden yeniye</summary>
    public List<BudgetPeriodDto> History { get; set; } = new();
}

public class BudgetPeriodDto
{
    public DateTime PeriodStart { get; set; }
    public DateTime PeriodEnd { get; set; }
    public decimal SpentAmount { get; set; }
}
