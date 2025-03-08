using ButceYonet.Application.Domain.Entities;
using ButceYonet.Application.Domain.Events;
using ButceYonet.Application.Infrastructure.Data;
using DotBoil.EFCore;
using DotBoil.MassTransit.Attributes;
using DotBoil.MassTransit.Consumers;
using MassTransit;
using Microsoft.EntityFrameworkCore;

namespace ButceYonet.Consumers;

[Consumer("transaction-label-added")]
public class TransactionLabelAddedDomainEventConsumer : BaseConsumer<TransactionLabelAddedDomainEvent>
{
    private readonly IServiceProvider _serviceProvider;
    private IRepository<Transaction, ButceYonetDbContext> _transactionRepository;
    private IRepository<CategorizedTransactionReport, ButceYonetDbContext> _categorizedTransactionReportRepository;
    
    public TransactionLabelAddedDomainEventConsumer(IServiceProvider serviceProvider) : base(serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public override async Task ConsumeEvent(ConsumeContext<TransactionLabelAddedDomainEvent> context)
    {
        using var scope = _serviceProvider.CreateScope();
        InitializeDependencies(scope);

        await Task.WhenAll(
            ProcessCategorizedTransactionReport(context.Message));

        await _categorizedTransactionReportRepository.SaveChangesAsync();
    }

    private void InitializeDependencies(IServiceScope serviceScope)
    {
        _transactionRepository = serviceScope
            .ServiceProvider
            .GetRequiredService<IRepository<Transaction, ButceYonetDbContext>>();
        
        _categorizedTransactionReportRepository = serviceScope
            .ServiceProvider
            .GetRequiredService<IRepository<CategorizedTransactionReport, ButceYonetDbContext>>();
    }

    private async Task ProcessCategorizedTransactionReport(TransactionLabelAddedDomainEvent domainEvent)
    {
        var transaction = await _transactionRepository
            .Get()
            .Where(t => t.Id == domainEvent.TransactionId)
            .FirstOrDefaultAsync();

        if (transaction is null)
            return;
        
        var transactionDate = transaction.TransactionDate;
        var reportDate = new DateTime(transactionDate.Year, transactionDate.Month, transactionDate.Day, 0, 0,0);

        var categorizedTransactionReport = await _categorizedTransactionReportRepository
            .Get()
            .Where(p =>
                p.NotebookId == transaction.NotebookId &&
                p.TransactionType == transaction.TransactionType &&
                p.CurrencyId == transaction.CurrencyId &&
                p.Term == reportDate &&
                p.NotebookLabelId == domainEvent.NotebookLabelId)
            .FirstOrDefaultAsync();

        if (categorizedTransactionReport is null)
        {
            categorizedTransactionReport = new CategorizedTransactionReport
            {
                NotebookId = transaction.NotebookId.Value,
                TransactionType = transaction.TransactionType,
                CurrencyId = transaction.CurrencyId,
                Term = reportDate,
                NotebookLabelId = domainEvent.NotebookLabelId
            };
        }

        categorizedTransactionReport.Amount += transaction.Amount;

        if (categorizedTransactionReport.Id == default(int))
            await _categorizedTransactionReportRepository.AddAsync(categorizedTransactionReport);
        else
            _categorizedTransactionReportRepository.Update(categorizedTransactionReport);
    }
}