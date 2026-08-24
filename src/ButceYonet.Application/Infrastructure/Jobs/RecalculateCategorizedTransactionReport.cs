using ButceYonet.Application.Domain.Entities;
using ButceYonet.Application.Domain.Enums;
using ButceYonet.Application.Infrastructure.Data;
using DotBoil.EFCore;
using DotBoil.Parameter;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace ButceYonet.Application.Infrastructure.Jobs;

public class RecalculateCategorizedTransactionReport : BackgroundService
{
    private readonly IServiceProvider _serviceProvider;

    public RecalculateCategorizedTransactionReport(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        using var scope = _serviceProvider.CreateScope();
        var sp = scope.ServiceProvider;

        var parameterManager = sp.GetRequiredService<IParameterManager>();
        var isActive = await parameterManager.GetParameterValue<bool>(nameof(RecalculateCategorizedTransactionReport), "IsActive");

        if (!isActive)
            return;

        var termStart = await parameterManager.GetParameterValue<DateTime>(nameof(RecalculateCategorizedTransactionReport), "TermStart");
        var termEnd = await parameterManager.GetParameterValue<DateTime>(nameof(RecalculateCategorizedTransactionReport), "TermEnd");

        var reportRepository = sp.GetRequiredService<IRepository<CategorizedTransactionReportV2, ButceYonetDbContext>>();
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
            .Include(t => t.TransactionLabelsV2)
            .ToListAsync(stoppingToken);

        var rows = new List<(int NotebookV2Id, int UserLabelId, TransactionTypes TransactionType, int CurrencyId, DateTime Term, decimal Amount)>();

        // Gerçek zamanlı consumer'lar (TransactionReportSyncService) Term'i gün bazında yazıyor;
        // tutarlı kalmak ve gelecekte çift sayımı önlemek için burada da aynı granülerlik kullanılır.
        foreach (var transaction in transactions)
        {
            var term = transaction.TransactionDate.Date;

            foreach (var label in transaction.TransactionLabelsV2.Where(l => !l.IsDeleted))
            {
                rows.Add((transaction.NotebookV2Id, label.UserLabelId, transaction.TransactionType, transaction.CurrencyId, term, transaction.Amount));
            }
        }

        var grouped = rows.GroupBy(r => new { r.NotebookV2Id, r.UserLabelId, r.TransactionType, r.CurrencyId, r.Term });

        foreach (var g in grouped)
        {
            await reportRepository.AddAsync(new CategorizedTransactionReportV2
            {
                NotebookV2Id = g.Key.NotebookV2Id,
                UserLabelId = g.Key.UserLabelId,
                TransactionType = g.Key.TransactionType,
                CurrencyId = g.Key.CurrencyId,
                Term = g.Key.Term,
                Amount = g.Sum(x => x.Amount)
            });
        }

        await reportRepository.SaveChangesAsync();
    }
}
