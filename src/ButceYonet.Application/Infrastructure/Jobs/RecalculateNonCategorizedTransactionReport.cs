using ButceYonet.Application.Domain.Entities;
using ButceYonet.Application.Infrastructure.Data;
using DotBoil.EFCore;
using DotBoil.Parameter;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace ButceYonet.Application.Infrastructure.Jobs;

public class RecalculateNonCategorizedTransactionReport : BackgroundService
{
    private readonly IServiceProvider _serviceProvider;

    public RecalculateNonCategorizedTransactionReport(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        using var scope = _serviceProvider.CreateScope();
        var sp = scope.ServiceProvider;

        var parameterManager = sp.GetRequiredService<IParameterManager>();
        var isActive = await parameterManager.GetParameterValue<bool>(nameof(RecalculateNonCategorizedTransactionReport), "IsActive");

        if (!isActive)
            return;

        var termStart = await parameterManager.GetParameterValue<DateTime>(nameof(RecalculateNonCategorizedTransactionReport), "TermStart");
        var termEnd = await parameterManager.GetParameterValue<DateTime>(nameof(RecalculateNonCategorizedTransactionReport), "TermEnd");

        var reportRepository = sp.GetRequiredService<IRepository<NonCategorizedTransactionReport, ButceYonetDbContext>>();
        var transactionRepository = sp.GetRequiredService<IRepository<TransactionV2, ButceYonetDbContext>>();

        var existingReports = await reportRepository
            .GetAll()
            .Where(r => r.Term >= termStart && r.Term <= termEnd)
            .ToListAsync(stoppingToken);

        reportRepository.RemoveRange(existingReports);
        await reportRepository.SaveChangesAsync();

        var transactions = await transactionRepository
            .GetAll()
            .Where(t =>
                t.IsMatched &&
                t.TransactionDate >= termStart &&
                t.TransactionDate <= termEnd)
            .ToListAsync(stoppingToken);

        // Gerçek zamanlı consumer'lar (TransactionReportSyncService) Term'i gün bazında yazıyor;
        // tutarlı kalmak ve gelecekte çift sayımı önlemek için burada da aynı granülerlik kullanılır.
        var grouped = transactions
            .GroupBy(t => new
            {
                t.NotebookV2Id,
                t.TransactionType,
                t.CurrencyId,
                Term = t.TransactionDate.Date
            });

        foreach (var g in grouped)
        {
            await reportRepository.AddAsync(new NonCategorizedTransactionReport
            {
                NotebookV2Id = g.Key.NotebookV2Id,
                TransactionType = g.Key.TransactionType,
                CurrencyId = g.Key.CurrencyId,
                Term = g.Key.Term,
                Amount = g.Sum(x => x.Amount)
            });
        }

        await reportRepository.SaveChangesAsync();
    }
}
