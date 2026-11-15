namespace ButceYonet.Application.Application.Shared;

/// <summary>
/// Aylık bütçe dönemi hesabı. Dönem [Start, End] kapalı aralıktır ve gün bazındadır (saat bilgisi yok),
/// rapor tablolarındaki Term alanıyla doğrudan karşılaştırılabilir.
/// </summary>
public static class BudgetPeriodCalculator
{
    public const int MinStartDay = 1;
    public const int MaxStartDay = 28;

    /// <summary>
    /// asOf tarihini içeren dönemi döner.
    /// Örn. startDay = 15: 20 Mart → 15 Mart - 14 Nisan, 10 Mart → 15 Şubat - 14 Mart.
    /// </summary>
    public static (DateTime Start, DateTime End) GetPeriod(int startDay, DateTime asOf)
    {
        startDay = Math.Clamp(startDay, MinStartDay, MaxStartDay);
        var date = asOf.Date;

        var start = new DateTime(date.Year, date.Month, startDay);
        if (date.Day < startDay)
            start = start.AddMonths(-1);

        return (start, start.AddMonths(1).AddDays(-1));
    }

    /// <summary>
    /// asOf'u içeren dönem dahil, geriye doğru `count` adet dönemi eskiden yeniye sıralı döner.
    /// </summary>
    public static List<(DateTime Start, DateTime End)> GetPeriods(int startDay, DateTime asOf, int count)
    {
        var (currentStart, _) = GetPeriod(startDay, asOf);

        return Enumerable.Range(0, count)
            .Select(i => currentStart.AddMonths(-(count - 1 - i)))
            .Select(start => (start, start.AddMonths(1).AddDays(-1)))
            .ToList();
    }

    /// <summary>
    /// Bugün dahil dönemin bitimine kalan gün sayısı (dönemin son günü için 1).
    /// </summary>
    public static int GetDaysLeft(DateTime periodEnd, DateTime asOf)
    {
        return Math.Max(0, (periodEnd.Date - asOf.Date).Days + 1);
    }
}
