using ButceYonet.Application.Domain.Entities;

namespace ButceYonet.Application.Application.Interfaces;

public interface ITransactionReportSyncService
{
    /// <summary>
    /// NonCategorizedTransactionReport toplamını verilen tutar kadar günceller (pozitif = ekleme, negatif = çıkarma).
    /// Kendi scope/DbContext'ini açar, bağımsız ve eşzamanlı çağrılabilir.
    /// </summary>
    Task SyncNonCategorizedAsync(TransactionV2 transaction, decimal amountDelta, CancellationToken cancellationToken = default);

    /// <summary>
    /// CategorizedTransactionReportV2 toplamlarını (işlemin her etiketi için) verilen tutar kadar günceller.
    /// Kendi scope/DbContext'ini açar, bağımsız ve eşzamanlı çağrılabilir.
    /// </summary>
    Task SyncCategorizedAsync(TransactionV2 transaction, decimal amountDelta, CancellationToken cancellationToken = default);
}
