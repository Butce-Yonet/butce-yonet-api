using ButceYonet.Application.Domain.Entities;

namespace ButceYonet.Application.Application.Interfaces;

public interface IBudgetSignalService
{
    /// <summary>
    /// Bir işlem eklendikten/güncellendikten ve rapor tabloları senkronlandıktan SONRA çağrılır.
    /// İşlemin etkilediği bütçelerde %80 / %100 eşiği bu işlemle ilk kez aşıldıysa sinyal üretir.
    /// Güncellemede previousTransaction verilir; eski halinin katkısı düşülerek "önceki" harcama bulunur.
    /// Hata fırlatmaz (loglar): rapor senkronu tamamlandıktan sonra consumer'ın retry'a düşüp raporları
    /// ikinci kez senkronlamasına sebep olmamalı.
    /// </summary>
    Task EvaluateAsync(TransactionV2 transaction, TransactionV2 previousTransaction = null, CancellationToken cancellationToken = default);
}
