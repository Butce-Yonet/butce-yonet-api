using ButceYonet.Application.Domain.Entities;

namespace ButceYonet.Application.Application.Interfaces;

public interface IBudgetSpendingCalculator
{
    /// <summary>
    /// Kullanıcının [from, to] aralığındaki günlük gider toplamlarını rapor tablolarından tek seferde yükler.
    /// Dönen snapshot üzerinden, aralığa düşen herhangi bir bütçe/dönem için harcama ek sorgu atmadan hesaplanır.
    /// </summary>
    Task<IBudgetSpendingSnapshot> LoadAsync(int userId, DateTime from, DateTime to, CancellationToken cancellationToken = default);
}

public interface IBudgetSpendingSnapshot
{
    /// <summary>
    /// Etiketli bütçede o etiketin, genel bütçede (UserLabelId null) etiketten bağımsız tüm giderlerin
    /// bütçenin para birimindeki [start, end] toplamı. Genel bütçe NonCategorized rapordan okunduğu için
    /// birden fazla etiketli bir işlem iki kez sayılmaz.
    /// </summary>
    decimal GetSpent(Budget budget, DateTime start, DateTime end);
}
