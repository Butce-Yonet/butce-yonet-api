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

    private IRepository<TransactionLabelV2, ButceYonetDbContext> _transactionLabelV2Repository;
    private IRepository<NonCategorizedTransactionReport, ButceYonetDbContext> _nonCategorizedTransactionReportRepository;
    private IRepository<CategorizedTransactionReportV2, ButceYonetDbContext> _categorizedTransactionReportRepository;

    public TransactionDeletedDomainEventConsumer(IServiceProvider serviceProvider) : base(serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public override async Task ConsumeEvent(ConsumeContext<TransactionDeletedDomainEvent> context)
    {
        if (!context.Message.Transaction.IsMatched)
            return;

        if (!context.Message.Transaction.IsProceed)
            return;

        using var scope = _serviceProvider.CreateScope();
        InitializeDependencies(scope);

        await ProcessNonCategorizedTransactionReport(context.Message);
        await ProcessCategorizedTransactionReport(context.Message);

        await _nonCategorizedTransactionReportRepository.SaveChangesAsync();
    }

    private void InitializeDependencies(IServiceScope scope)
    {
        _transactionLabelV2Repository = scope.ServiceProvider.GetService<IRepository<TransactionLabelV2, ButceYonetDbContext>>();

        _nonCategorizedTransactionReportRepository = scope.ServiceProvider
            .GetRequiredService<IRepository<NonCategorizedTransactionReport, ButceYonetDbContext>>();

        _categorizedTransactionReportRepository = scope.ServiceProvider
            .GetRequiredService<IRepository<CategorizedTransactionReportV2, ButceYonetDbContext>>();
    }

    private async Task ProcessNonCategorizedTransactionReport(TransactionDeletedDomainEvent domainEvent)
    {
        var transactionDate = domainEvent.Transaction.TransactionDate;
        var reportDate = new DateTime(transactionDate.Year, transactionDate.Month, transactionDate.Day, 0, 0, 0);

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
        var reportDate = new DateTime(transactionDate.Year, transactionDate.Month, transactionDate.Day, 0, 0, 0);

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
            _transactionLabelV2Repository
                .GetAll()
                .Where(tl => tl.TransactionId == domainEvent.Transaction.Id)
                .ToListAsync();

        foreach (var transactionLabel in transactionLabels.Where(tl => !tl.IsDeleted))
        {
            var categorizedTransactionReport = categorizedTransactionReports
                .FirstOrDefault(item => item.UserLabelId == transactionLabel.UserLabelId);

            if (categorizedTransactionReport != null)
            {
                categorizedTransactionReport.Amount -= domainEvent.Transaction.Amount;
                _categorizedTransactionReportRepository.Update(categorizedTransactionReport);
            }
        }
    }
}