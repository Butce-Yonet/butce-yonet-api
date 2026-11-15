namespace ButceYonet.Application.Domain.Enums;

public enum BudgetStatus
{
    /// <summary>Harcama limitin %80'inin altında</summary>
    OnTrack = 1,

    /// <summary>Harcama limitin %80'i ile %100'ü arasında</summary>
    Warning = 2,

    /// <summary>Harcama limite ulaştı ya da aştı</summary>
    Exceeded = 3
}
