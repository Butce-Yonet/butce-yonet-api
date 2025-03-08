using ButceYonet.Application.Domain.Entities;
using ButceYonet.Application.Domain.Events;
using ButceYonet.Application.Infrastructure.Data;
using DotBoil.EFCore;
using DotBoil.MassTransit.Attributes;
using DotBoil.MassTransit.Consumers;
using MassTransit;
using Microsoft.EntityFrameworkCore;

namespace ButceYonet.Consumers;

[Consumer("transaction-deleted")]
public class TransactionDeletedDomainEventConsumer : BaseConsumer<TransactionDeletedDomainEvent>
{
    private readonly IServiceProvider _serviceProvider;

    private IRepository<TransactionLabel, ButceYonetDbContext> _transactionLabelRepository;
    private IRepository<NonCategorizedTransactionReport, ButceYonetDbContext>
        _nonCategorizedTransactionReportRepository;
    private IRepository<CategorizedTransactionReport, ButceYonetDbContext> 
        _categorizedTransactionReportRepository;
    
    public TransactionDeletedDomainEventConsumer(IServiceProvider serviceProvider) : base(serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public override async Task ConsumeEvent(ConsumeContext<TransactionDeletedDomainEvent> context)
    {
        using var scope = _serviceProvider.CreateScope();
        InitializeDependencies(scope);

        await Task.WhenAll(
            ProcessNonCategorizedTransactionReport(context.Message),
            ProcessCategorizedTransactionReport(context.Message));

        await _nonCategorizedTransactionReportRepository.SaveChangesAsync();
    }

    private void InitializeDependencies(IServiceScope scope)
    {
        _transactionLabelRepository = scope.ServiceProvider.GetService<IRepository<TransactionLabel, ButceYonetDbContext>>();
        
        _nonCategorizedTransactionReportRepository = scope.ServiceProvider
            .GetRequiredService<IRepository<NonCategorizedTransactionReport, ButceYonetDbContext>>();

        _categorizedTransactionReportRepository = scope.ServiceProvider
            .GetRequiredService<IRepository<CategorizedTransactionReport, ButceYonetDbContext>>();
    }

    private async Task ProcessNonCategorizedTransactionReport(TransactionDeletedDomainEvent domainEvent)
    {
        var transactionDate = domainEvent.Transaction.TransactionDate;
        var reportDate = new DateTime(transactionDate.Year, transactionDate.Month, transactionDate.Day, 0, 0,0);

        var nonCategorizedTransactionReport = await
            _nonCategorizedTransactionReportRepository
                .Get()
                .Where(p => 
                    p.NotebookId == domainEvent.Transaction.NotebookId &&
                    p.TransactionType == domainEvent.Transaction.TransactionType &&
                    p.CurrencyId == domainEvent.Transaction.CurrencyId &&
                    p.Term == reportDate)
                .FirstOrDefaultAsync();

        if (nonCategorizedTransactionReport != null)
        {
            nonCategorizedTransactionReport.Amount -= domainEvent.Transaction.Amount;
            
            _nonCategorizedTransactionReportRepository.Update(nonCategorizedTransactionReport);
        }
    }

    private async Task ProcessCategorizedTransactionReport(TransactionDeletedDomainEvent domainEvent)
    {
        var transactionDate = domainEvent.Transaction.TransactionDate;
        var reportDate = new DateTime(transactionDate.Year, transactionDate.Month, transactionDate.Day, 0, 0,0);

        var categorizedTransactionReports = await
            _categorizedTransactionReportRepository
                .GetAll()
                .Where(p =>
                    p.NotebookId == domainEvent.Transaction.NotebookId &&
                    p.TransactionType == domainEvent.Transaction.TransactionType &&
                    p.CurrencyId == domainEvent.Transaction.CurrencyId &&
                    p.Term == reportDate)
                .ToListAsync();

        var transactionLabels = await
            _transactionLabelRepository
                .GetAll()
                .Where(tl => tl.TransactionId == domainEvent.Transaction.Id)
                .ToListAsync();

        foreach (var transactionLabel in transactionLabels)
        {
            var categorizedTransactionReport = categorizedTransactionReports
                .Where(item => item.NotebookLabelId == transactionLabel.NotebookLabelId)
                .FirstOrDefault();

            if (categorizedTransactionReport != null)
            {
                categorizedTransactionReport.Amount -= domainEvent.Transaction.Amount;
                _categorizedTransactionReportRepository.Update(categorizedTransactionReport);
            }
        }
    }
}