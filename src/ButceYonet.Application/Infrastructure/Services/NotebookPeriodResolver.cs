using System.Globalization;
using ButceYonet.Application.Application.Interfaces;
using ButceYonet.Application.Domain.Entities;
using ButceYonet.Application.Infrastructure.Data;
using DotBoil.EFCore;
using Microsoft.EntityFrameworkCore;

namespace ButceYonet.Application.Infrastructure.Services;

public class NotebookPeriodResolver : INotebookPeriodResolver
{
    private static readonly CultureInfo TurkishCulture = new("tr-TR");

    private readonly IRepository<NotebookV2, ButceYonetDbContext> _notebookV2Repository;

    public NotebookPeriodResolver(IRepository<NotebookV2, ButceYonetDbContext> notebookV2Repository)
    {
        _notebookV2Repository = notebookV2Repository;
    }

    public async Task<NotebookV2> ResolveOrCreateAsync(int userId, DateTime date, CancellationToken cancellationToken = default)
    {
        var termStart = new DateTime(date.Year, date.Month, 1);
        var termEnd = termStart.AddMonths(1).AddTicks(-1);

        var existing = await _notebookV2Repository
            .Get()
            .Where(n => n.UserId == userId && n.TermStart == termStart)
            .FirstOrDefaultAsync(cancellationToken);

        if (existing is not null)
            return existing;

        var notebook = new NotebookV2
        {
            UserId = userId,
            TermStart = termStart,
            TermEnd = termEnd,
            Name = FormatTurkishMonthYear(termStart)
        };

        try
        {
            await _notebookV2Repository.AddAsync(notebook);
            await _notebookV2Repository.SaveChangesAsync();
            return notebook;
        }
        catch (DbUpdateException)
        {
            var winner = await _notebookV2Repository
                .Get()
                .Where(n => n.UserId == userId && n.TermStart == termStart)
                .FirstOrDefaultAsync(cancellationToken);

            if (winner is null)
                throw;

            return winner;
        }
    }

    private static string FormatTurkishMonthYear(DateTime termStart)
    {
        return termStart.ToString("MMMM yyyy", TurkishCulture);
    }
}
