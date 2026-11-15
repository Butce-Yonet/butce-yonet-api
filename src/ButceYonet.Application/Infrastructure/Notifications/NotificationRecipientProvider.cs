using ButceYonet.Application.Domain.Entities;
using ButceYonet.Application.Infrastructure.Data;
using DotBoil.EFCore;
using Microsoft.EntityFrameworkCore;
using User = ButceYonet.Application.Domain.Entities.User;

namespace ButceYonet.Application.Infrastructure.Notifications;

public class NotificationRecipientProvider : INotificationRecipientProvider
{
    private readonly IRepository<User, ButceYonetAuthorizationDbContext> _userRepository;
    private readonly IRepository<UserNotificationPreference, ButceYonetAuthorizationDbContext> _preferenceRepository;

    public NotificationRecipientProvider(
        IRepository<User, ButceYonetAuthorizationDbContext> userRepository,
        IRepository<UserNotificationPreference, ButceYonetAuthorizationDbContext> preferenceRepository)
    {
        _userRepository = userRepository;
        _preferenceRepository = preferenceRepository;
    }

    public async Task<NotificationRecipient?> GetAsync(int userId, CancellationToken cancellationToken = default)
    {
        var recipients = await LoadAsync(new[] { userId }, includePassive: false, cancellationToken);
        return recipients.GetValueOrDefault(userId);
    }

    public async Task<NotificationRecipient?> GetByEmailAsync(string email, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(email))
            return null;

        var userId = await _userRepository
            .Get()
            .Where(u => u.Email == email)
            .Select(u => (int?)u.Id)
            .FirstOrDefaultAsync(cancellationToken);

        if (!userId.HasValue)
            return null;

        // İşlemsel mailler (şifre sıfırlama vb.) için kullanılır: pasif kullanıcı da dahil.
        var recipients = await LoadAsync(new[] { userId.Value }, includePassive: true, cancellationToken);
        return recipients.GetValueOrDefault(userId.Value);
    }

    public Task<IReadOnlyDictionary<int, NotificationRecipient>> GetManyAsync(IEnumerable<int> userIds, CancellationToken cancellationToken = default)
    {
        return LoadAsync(userIds, includePassive: false, cancellationToken);
    }

    private async Task<IReadOnlyDictionary<int, NotificationRecipient>> LoadAsync(
        IEnumerable<int> userIds,
        bool includePassive,
        CancellationToken cancellationToken)
    {
        var ids = userIds.Distinct().ToList();
        if (ids.Count == 0)
            return new Dictionary<int, NotificationRecipient>();

        var users = await _userRepository
            .Get()
            .Where(u => ids.Contains(u.Id) && (includePassive || !u.IsPassive))
            .Select(u => new { u.Id, u.Name, u.Surname, u.Email, u.ActiveLanguage })
            .ToListAsync(cancellationToken);

        var enabledChannels = await _preferenceRepository
            .Get()
            .Where(p => ids.Contains(p.UserId) && p.IsEnabled)
            .Select(p => new { p.UserId, p.Channel })
            .ToListAsync(cancellationToken);

        var channelsByUser = enabledChannels
            .GroupBy(p => p.UserId)
            .ToDictionary(
                g => g.Key,
                g => (IReadOnlySet<string>)g.Select(p => p.Channel).ToHashSet(StringComparer.OrdinalIgnoreCase));

        return users.ToDictionary(
            u => u.Id,
            u => new NotificationRecipient
            {
                UserId = u.Id,
                Email = u.Email,
                DisplayName = string.IsNullOrWhiteSpace(u.Name) ? u.Email : $"{u.Name} {u.Surname}".Trim(),
                Language = NotificationLanguage.Normalize(u.ActiveLanguage),
                EnabledChannels = channelsByUser.GetValueOrDefault(u.Id) ?? new HashSet<string>(StringComparer.OrdinalIgnoreCase)
            });
    }
}
