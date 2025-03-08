using ButceYonet.Application.Domain.Entities;
using ButceYonet.Application.Domain.Events;
using ButceYonet.Application.Infrastructure.Data;
using DotBoil.EFCore;
using DotBoil.MassTransit.Attributes;
using DotBoil.MassTransit.Consumers;
using MassTransit;
using System.Linq;
using Microsoft.EntityFrameworkCore;

namespace ButceYonet.Consumers;

[Consumer("transaction-created")]
public class TransactionCreatedDomainEventConsumer : BaseConsumer<TransactionCreatedDomainEvent>
{
    private readonly IServiceProvider _serviceProvider;
    private IRepository<NonCategorizedTransactionReport, ButceYonetDbContext> _nonCategorizedTransactionReportRepository;
    private IRepository<CategorizedTransactionReport, ButceYonetDbContext> _categorizedTransactionReportRepository;
    
    public TransactionCreatedDomainEventConsumer(IServiceProvider serviceProvider) : base(serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public override async Task ConsumeEvent(ConsumeContext<TransactionCreatedDomainEvent> context)
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
        _nonCategorizedTransactionReportRepository = scope.ServiceProvider
            .GetRequiredService<IRepository<NonCategorizedTransactionReport, ButceYonetDbContext>>();
        _categorizedTransactionReportRepository = scope.ServiceProvider
            .GetRequiredService<IRepository<CategorizedTransactionReport, ButceYonetDbContext>>();
    }

    private async Task ProcessNonCategorizedTransactionReport(TransactionCreatedDomainEvent domainEvent)
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

        if (nonCategorizedTransactionReport is null)
        {
            nonCategorizedTransactionReport = new NonCategorizedTransactionReport()
            {
                NotebookId = domainEvent.Transaction.NotebookId.Value,
                TransactionType = domainEvent.Transaction.TransactionType,
                CurrencyId = domainEvent.Transaction.CurrencyId,
                Term = reportDate
            };
        }

        nonCategorizedTransactionReport.Amount += domainEvent.Transaction.Amount;

        if (nonCategorizedTransactionReport.Id == default(int))
            await _nonCategorizedTransactionReportRepository.AddAsync(nonCategorizedTransactionReport);
        else
            _nonCategorizedTransactionReportRepository.Update(nonCategorizedTransactionReport);
    }

    private async Task ProcessCategorizedTransactionReport(TransactionCreatedDomainEvent domainEvent)
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

        foreach (var transactionLabel in domainEvent.Transaction.TransactionLabels)
        {
            var categorizedTransactionReport =
                categorizedTransactionReports.FirstOrDefault(p =>
                    p.NotebookLabelId == transactionLabel.NotebookLabelId);

            if (categorizedTransactionReport is null)
            {
                categorizedTransactionReport = new CategorizedTransactionReport
                {
                    NotebookId = domainEvent.Transaction.NotebookId.Value,
                    NotebookLabelId = transactionLabel.NotebookLabelId,
                    TransactionType = domainEvent.Transaction.TransactionType,
                    CurrencyId = domainEvent.Transaction.CurrencyId,
                    Term = reportDate
                };
            }

            categorizedTransactionReport.Amount += domainEvent.Transaction.Amount;
            
            if (categorizedTransactionReport.Id == default(int))
                await _categorizedTransactionReportRepository.AddAsync(categorizedTransactionReport);
            else
                _categorizedTransactionReportRepository.Update(categorizedTransactionReport);
        }        
    }
}