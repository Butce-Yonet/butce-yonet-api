using DotBoil.Entities;

namespace ButceYonet.Application.Domain.Entities;

/// <summary>
/// Aylık harcama limiti. UserLabelId null ise etiketten bağımsız "genel" bütçedir (tüm giderler).
/// Harcama DB'de tutulmaz; dönem aralığına göre rapor tablolarından hesaplanır (bkz. IBudgetSpendingCalculator).
/// </summary>
public class Budget : BaseEntity
{
    public int UserId { get; set; }
    public int? UserLabelId { get; set; }
    public decimal Amount { get; set; }
    public int CurrencyId { get; set; }

    /// <summary>
    /// Dönemin başladığı ayın günü (1-28). Örn. maaşı ayın 15'inde yatan kullanıcı için 15 →
    /// dönem 15 Ocak - 14 Şubat. 28 ile sınırlı olması şubatta dönemin kaymasını engeller.
    /// </summary>
    public int PeriodStartDay { get; set; } = 1;

    public virtual UserLabel UserLabel { get; set; }
    public virtual Currency Currency { get; set; }
}
