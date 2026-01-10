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
    private IRepository<Transaction, ButceYonetDbContext> _transactionRepository;
    private IRepository<NonCategorizedTransactionReport, ButceYonetDbContext> _nonCategorizedTransactionReportRepository;
    private IRepository<CategorizedTransactionReport, ButceYonetDbContext> _categorizedTransactionReportRepository;
    
    public TransactionCreatedDomainEventConsumer(IServiceProvider serviceProvider) : base(serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public override async Task ConsumeEvent(ConsumeContext<TransactionCreatedDomainEvent> context)
    {
        if (!context.Message.Transaction.IsMatched)
            return;
        
        using var scope = _serviceProvider.CreateScope();
        InitializeDependencies(scope);

        var transaction = await
            _transactionRepository
                .Get()
                .Where(t => t.Id == context.Message.Transaction.Id)
                .FirstOrDefaultAsync();

        if (transaction is null)
            return;

        await ProcessNonCategorizedTransactionReport(context.Message);
        await ProcessCategorizedTransactionReport(context.Message);

        transaction.IsProceed = true;
        _transactionRepository.Update(transaction);

        await _nonCategorizedTransactionReportRepository.SaveChangesAsync();
    }

    private void InitializeDependencies(IServiceScope scope)
    {
        _transactionRepository =
            scope.ServiceProvider.GetRequiredService<IRepository<Transaction, ButceYonetDbContext>>();
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

        foreach (var transactionLabel in domainEvent.Transaction.TransactionLabels.Where(tl => !tl.IsDeleted))
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