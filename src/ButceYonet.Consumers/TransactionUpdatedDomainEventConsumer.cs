using System.Collections.Concurrent;
using ButceYonet.Application.Domain.Entities;
using ButceYonet.Application.Domain.Events;
using ButceYonet.Application.Infrastructure.Data;
using DotBoil.EFCore;
using DotBoil.MassTransit;
using DotBoil.MassTransit.Attributes;
using DotBoil.MassTransit.Consumers;
using MassTransit;
using Microsoft.EntityFrameworkCore;

namespace ButceYonet.Consumers;

[Consumer("transaction-updated")]
public class TransactionUpdatedDomainEventConsumer : BaseConsumer<TransactionUpdatedDomainEvent>
{
    private readonly IServiceProvider _serviceProvider;
    private IRepository<NonCategorizedTransactionReport, ButceYonetDbContext> _nonCategorizedTransactionReportRepository;
    private IRepository<CategorizedTransactionReport, ButceYonetDbContext> _categorizedTransactionReportRepository;

    private ConcurrentDictionary<DateTime, NonCategorizedTransactionReport> _nonCategorizedTransactionReportDictionary;
    private ConcurrentBag<CategorizedTransactionReport> _categorizedTransactionReportBag;
    
    public TransactionUpdatedDomainEventConsumer(IServiceProvider serviceProvider) : base(serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public override async Task ConsumeEvent(ConsumeContext<TransactionUpdatedDomainEvent> context)
    {
        if (!context.Message.OldTransaction.IsMatched)
            return;

        if (!context.Message.NewTransaction.IsProceed)
            return;
        
        using var scope = _serviceProvider.CreateScope();
        InitializeDependencies(scope);

        await ProcessNonCategorizedTransactionReport(context.Message.OldTransaction, true);
        await ProcessCategorizedTransactionReport(context.Message.OldTransaction, true);
        
        await ProcessNonCategorizedTransactionReport(context.Message.NewTransaction, false);
        await ProcessCategorizedTransactionReport(context.Message.NewTransaction, false);

        foreach (var nonCategorizedTransactionReport in _nonCategorizedTransactionReportDictionary.Values)
        {
            if (nonCategorizedTransactionReport.Id == default(int))
                await _nonCategorizedTransactionReportRepository.AddAsync(nonCategorizedTransactionReport);
            else
                _nonCategorizedTransactionReportRepository.Update(nonCategorizedTransactionReport);
        }

        foreach (var categorizedTransactionReport in _categorizedTransactionReportBag)
        {
            if (categorizedTransactionReport.Id == default(int))
                await _categorizedTransactionReportRepository.AddAsync(categorizedTransactionReport);
            else
                _categorizedTransactionReportRepository.Update(categorizedTransactionReport);
        }
        
        await _nonCategorizedTransactionReportRepository.SaveChangesAsync();
    }

    private void InitializeDependencies(IServiceScope serviceScope)
    {
        _nonCategorizedTransactionReportRepository = serviceScope.ServiceProvider.GetRequiredService<IRepository<NonCategorizedTransactionReport, ButceYonetDbContext>>();
        _categorizedTransactionReportRepository = serviceScope.ServiceProvider.GetRequiredService<IRepository<CategorizedTransactionReport, ButceYonetDbContext>>();
        _nonCategorizedTransactionReportDictionary = new ConcurrentDictionary<DateTime, NonCategorizedTransactionReport>();
        _categorizedTransactionReportBag = new ConcurrentBag<CategorizedTransactionReport>();
    }

    private async Task ProcessNonCategorizedTransactionReport(Transaction transaction, bool isOldTransaction)
    {
        var transactionDate = transaction.TransactionDate;
        var reportDate = new DateTime(transactionDate.Year, transactionDate.Month, transactionDate.Day, 0, 0,0);
        
        var nonCategorizedTransactionReport = default(NonCategorizedTransactionReport);

        if (_nonCategorizedTransactionReportDictionary.ContainsKey(reportDate))
        {
            nonCategorizedTransactionReport = _nonCategorizedTransactionReportDictionary[reportDate];
        }
        else
        {
            nonCategorizedTransactionReport = await _nonCategorizedTransactionReportRepository
                .Get()
                .Where(p =>
                    p.NotebookId == transaction.NotebookId &&
                    p.TransactionType == transaction.TransactionType &&
                    p.CurrencyId == transaction.CurrencyId &&
                    p.Term == reportDate)
                .FirstOrDefaultAsync();

            if (nonCategorizedTransactionReport is null)
            {
                nonCategorizedTransactionReport = new NonCategorizedTransactionReport
                {
                    NotebookId = transaction.NotebookId.Value,
                    TransactionType = transaction.TransactionType,
                    CurrencyId = transaction.CurrencyId,
                    Term = reportDate
                };
            }
            
            _nonCategorizedTransactionReportDictionary.TryAdd(reportDate, nonCategorizedTransactionReport);
        }
        
        if (isOldTransaction)
            nonCategorizedTransactionReport.Amount -= transaction.Amount;
        else
            nonCategorizedTransactionReport.Amount += transaction.Amount;
    }

    private async Task ProcessCategorizedTransactionReport(Transaction transaction, bool isOldTransaction)
    {
        var transactionDate = transaction.TransactionDate;
        var reportDate = new DateTime(transactionDate.Year, transactionDate.Month, transactionDate.Day, 0, 0,0);
        
        foreach(var transactionLabel in transaction.TransactionLabels.Where(tl => !tl.IsDeleted))
        {
            var categorizedTransactionReport = _categorizedTransactionReportBag
                .Where(item =>
                    item.NotebookId == transaction.NotebookId &&
                    item.NotebookLabelId == transactionLabel.NotebookLabelId &&
                    item.TransactionType == transaction.TransactionType &&
                    item.CurrencyId == transaction.CurrencyId &&
                    item.Term == reportDate)
                .FirstOrDefault();

            if (categorizedTransactionReport is null)
            {
                categorizedTransactionReport = await _categorizedTransactionReportRepository
                    .Get()
                    .Where(item =>
                        item.NotebookId == transaction.NotebookId &&
                        item.NotebookLabelId == transactionLabel.NotebookLabelId &&
                        item.TransactionType == transaction.TransactionType &&
                        item.CurrencyId == transaction.CurrencyId &&
                        item.Term == reportDate)
                    .FirstOrDefaultAsync();

                if (categorizedTransactionReport is null)
                {
                    categorizedTransactionReport = new CategorizedTransactionReport
                    {
                        NotebookId = transaction.NotebookId.Value,
                        NotebookLabelId = transactionLabel.NotebookLabelId,
                        TransactionType = transaction.TransactionType,
                        CurrencyId = transaction.CurrencyId,
                        Term = reportDate
                    };
                }
                
                _categorizedTransactionReportBag.Add(categorizedTransactionReport);
            }
            
            if (isOldTransaction)
                categorizedTransactionReport.Amount -= transaction.Amount;
            else
                categorizedTransactionReport.Amount += transaction.Amount;
        }
    }
}