using ButceYonet.Application.Application.Interfaces;
using ButceYonet.Application.Domain.Entities;
using ButceYonet.Application.Infrastructure.Data;
using DotBoil.EFCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace ButceYonet.Application.Infrastructure.Services;

public class TransactionReportSyncService : ITransactionReportSyncService
{
    private readonly IServiceProvider _serviceProvider;

    public TransactionReportSyncService(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public async Task SyncNonCategorizedAsync(TransactionV2 transaction, decimal amountDelta, CancellationToken cancellationToken = default)
    {
        // Kendi scope'unu (dolayısıyla kendi DbContext'ini) açar; SyncCategorizedAsync ile
        // aynı DbContext'i paylaşmadığı için Task.WhenAll ile güvenle paralel çalıştırılabilir.
        using var scope = _serviceProvider.CreateScope();
        var repository = scope.ServiceProvider
            .GetRequiredService<IRepository<NonCategorizedTransactionReport, ButceYonetDbContext>>();

        var term = ReportTerm(transaction.TransactionDate);

        var report = await repository
            .Get()
            .Where(p =>
                p.NotebookV2Id == transaction.NotebookV2Id &&
                p.TransactionType == transaction.TransactionType &&
                p.CurrencyId == transaction.CurrencyId &&
                p.Term == term)
            .FirstOrDefaultAsync(cancellationToken);

        var isNew = report is null;

        report ??= new NonCategorizedTransactionReport
        {
            NotebookV2Id = transaction.NotebookV2Id,
            TransactionType = transaction.TransactionType,
            CurrencyId = transaction.CurrencyId,
            Term = term
        };

        report.Amount += amountDelta;

        if (isNew)
            await repository.AddAsync(report);
        else
            repository.Update(report);

        await repository.SaveChangesAsync();
    }

    public async Task SyncCategorizedAsync(TransactionV2 transaction, decimal amountDelta, CancellationToken cancellationToken = default)
    {
        var labels = (transaction.TransactionLabelsV2 ?? new List<TransactionLabelV2>())
            .Where(tl => !tl.IsDeleted)
            .ToList();

        if (!labels.Any())
            return;

        using var scope = _serviceProvider.CreateScope();
        var repository = scope.ServiceProvider
            .GetRequiredService<IRepository<CategorizedTransactionReportV2, ButceYonetDbContext>>();

        var term = ReportTerm(transaction.TransactionDate);

        var existingReports = await repository
            .GetAll()
            .Where(p =>
                p.NotebookV2Id == transaction.NotebookV2Id &&
                p.TransactionType == transaction.TransactionType &&
                p.CurrencyId == transaction.CurrencyId &&
                p.Term == term)
            .ToListAsync(cancellationToken);

        foreach (var label in labels)
        {
            var report = existingReports.FirstOrDefault(p => p.UserLabelId == label.UserLabelId);
            var isNew = report is null;

            report ??= new CategorizedTransactionReportV2
            {
                NotebookV2Id = transaction.NotebookV2Id,
                UserLabelId = label.UserLabelId,
                TransactionType = transaction.TransactionType,
                CurrencyId = transaction.CurrencyId,
                Term = term
            };

            report.Amount += amountDelta;

            if (isNew)
                await repository.AddAsync(report);
            else
                repository.Update(report);
        }

        await repository.SaveChangesAsync();
    }

    private static DateTime ReportTerm(DateTime transactionDate)
    {
        return new DateTime(transactionDate.Year, transactionDate.Month, transactionDate.Day, 0, 0, 0);
    }
}
