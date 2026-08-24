using ButceYonet.Application.Domain.Entities;

namespace ButceYonet.Application.Application.Interfaces;

public interface INotebookPeriodResolver
{
    Task<NotebookV2> ResolveOrCreateAsync(int userId, DateTime date, CancellationToken cancellationToken = default);
}
