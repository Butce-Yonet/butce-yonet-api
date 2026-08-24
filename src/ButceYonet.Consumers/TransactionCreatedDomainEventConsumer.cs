using ButceYonet.Application.Application.Interfaces;
using ButceYonet.Application.Domain.Entities;
using ButceYonet.Application.Domain.Events;
using ButceYonet.Application.Infrastructure.Data;
using DotBoil.EFCore;
using DotBoil.MassTransit.Attributes;
using DotBoil.MassTransit.Consumers;
using MassTransit;
using Microsoft.EntityFrameworkCore;

namespace ButceYonet.Consumers;

[Consumer("transaction-created")]
public class TransactionCreatedDomainEventConsumer : BaseConsumer<TransactionCreatedDomainEvent>
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ITransactionReportSyncService _reportSyncService;

    public TransactionCreatedDomainEventConsumer(
        IServiceProvider serviceProvider,
        ITransactionReportSyncService reportSyncService) : base(serviceProvider)
    {
        _serviceProvider = serviceProvider;
        _reportSyncService = reportSyncService;
    }

    public override async Task ConsumeEvent(ConsumeContext<TransactionCreatedDomainEvent> context)
    {
        if (!context.Message.Transaction.IsMatched)
            return;

        using var scope = _serviceProvider.CreateScope();
        var transactionRepository = scope.ServiceProvider
            .GetRequiredService<IRepository<TransactionV2, ButceYonetDbContext>>();

        var transaction = await
            transactionRepository
                .Get()
                .Where(t => t.Id == context.Message.Transaction.Id)
                .Include(t => t.TransactionLabelsV2)
                .FirstOrDefaultAsync();

        if (transaction is null)
            return;

        await Task.WhenAll(
            _reportSyncService.SyncNonCategorizedAsync(context.Message.Transaction, context.Message.Transaction.Amount),
            _reportSyncService.SyncCategorizedAsync(context.Message.Transaction, context.Message.Transaction.Amount));

        transaction.IsProceed = true;
        transactionRepository.Update(transaction);
        await transactionRepository.SaveChangesAsync();
    }
}
