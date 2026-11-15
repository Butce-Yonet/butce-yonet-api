using ButceYonet.Application.Application.Interfaces;
using ButceYonet.Application.Domain.Entities;
using ButceYonet.Application.Domain.Events;
using DotBoil.MassTransit.Attributes;
using DotBoil.MassTransit.Consumers;
using MassTransit;
using Microsoft.Extensions.DependencyInjection;

namespace ButceYonet.Consumers;

[Consumer("transaction-updated")]
public class TransactionUpdatedDomainEventConsumer : BaseConsumer<TransactionUpdatedDomainEvent>
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ITransactionReportSyncService _reportSyncService;

    public TransactionUpdatedDomainEventConsumer(
        IServiceProvider serviceProvider,
        ITransactionReportSyncService reportSyncService) : base(serviceProvider)
    {
        _serviceProvider = serviceProvider;
        _reportSyncService = reportSyncService;
    }

    public override async Task ConsumeEvent(ConsumeContext<TransactionUpdatedDomainEvent> context)
    {
        if (!context.Message.OldTransaction.IsMatched)
            return;

        if (!context.Message.NewTransaction.IsProceed)
            return;

        var oldTransaction = context.Message.OldTransaction;
        var newTransaction = context.Message.NewTransaction;

        // Categorized ve NonCategorized birbirinden bağımsız tablolara yazdığı için
        // (ayrı scope/DbContext açtıklarından) paralel çalıştırılabilirler.
        // Her ikisinde de eski tutar düşülüp yeni tutar eklenir; sıralı iki çağrı
        // aynı bucket'a düşse bile birbirinin sonucunu görecek şekilde (SaveChanges her çağrıda tamamlanır) doğru netleşir.
        await Task.WhenAll(
            ProcessNonCategorizedAsync(oldTransaction, newTransaction),
            ProcessCategorizedAsync(oldTransaction, newTransaction));

        // Tutar artışı ya da etiket/tarih değişikliği bir bütçe eşiğini geçirebilir.
        using var scope = _serviceProvider.CreateScope();
        var budgetSignalService = scope.ServiceProvider.GetRequiredService<IBudgetSignalService>();
        await budgetSignalService.EvaluateAsync(newTransaction, oldTransaction);
    }

    private async Task ProcessNonCategorizedAsync(TransactionV2 oldTransaction, TransactionV2 newTransaction)
    {
        await _reportSyncService.SyncNonCategorizedAsync(oldTransaction, -oldTransaction.Amount);
        await _reportSyncService.SyncNonCategorizedAsync(newTransaction, newTransaction.Amount);
    }

    private async Task ProcessCategorizedAsync(TransactionV2 oldTransaction, TransactionV2 newTransaction)
    {
        await _reportSyncService.SyncCategorizedAsync(oldTransaction, -oldTransaction.Amount);
        await _reportSyncService.SyncCategorizedAsync(newTransaction, newTransaction.Amount);
    }
}
