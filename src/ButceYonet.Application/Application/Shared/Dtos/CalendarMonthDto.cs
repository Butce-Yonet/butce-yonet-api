namespace ButceYonet.Application.Application.Shared.Dtos;

/// <summary>
/// Belirli bir ay için takvim görünümü: ay geneli toplamlar ve içerdiği günlerin özetleri.
/// </summary>
public class CalendarMonthDto
{
    public int Year { get; set; }
    public int Month { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }

    /// <summary>Ay geneli toplam gelir</summary>
    public decimal TotalIncome { get; set; }

    /// <summary>Ay geneli toplam gider</summary>
    public decimal TotalExpense { get; set; }

    /// <summary>Ay geneli net bakiye (Gelir - Gider)</summary>
    public decimal NetBalance { get; set; }

    /// <summary>İşlem içeren günlerin özetleri (işlemi olmayan günler listede yer almaz)</summary>
    public List<CalendarDayDto> Days { get; set; } = new();
}
