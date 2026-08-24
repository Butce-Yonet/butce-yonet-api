using ButceYonet.Application.Application.Interfaces;
using ButceYonet.Application.Domain.Events;
using DotBoil.MassTransit.Attributes;
using DotBoil.MassTransit.Consumers;
using MassTransit;

namespace ButceYonet.Consumers;

[Consumer("transaction-deleted")]
public class TransactionDeletedDomainEventConsumer : BaseConsumer<TransactionDeletedDomainEvent>
{
    private readonly ITransactionReportSyncService _reportSyncService;

    public TransactionDeletedDomainEventConsumer(
        IServiceProvider serviceProvider,
        ITransactionReportSyncService reportSyncService) : base(serviceProvider)
    {
        _reportSyncService = reportSyncService;
    }

    public override async Task ConsumeEvent(ConsumeContext<TransactionDeletedDomainEvent> context)
    {
        if (!context.Message.Transaction.IsMatched)
            return;

        if (!context.Message.Transaction.IsProceed)
            return;

        var transaction = context.Message.Transaction;

        await Task.WhenAll(
            _reportSyncService.SyncNonCategorizedAsync(transaction, -transaction.Amount),
            _reportSyncService.SyncCategorizedAsync(transaction, -transaction.Amount));
    }
}
