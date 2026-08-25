using ButceYonet.Application.Application.Shared;
using ButceYonet.Application.Domain.Entities;
using ButceYonet.Application.Domain.Enums;
using ButceYonet.Application.Infrastructure.Data;
using ButceYonet.Application.Infrastructure.MailTemplates;
using DotBoil;
using DotBoil.Configuration;
using DotBoil.Email;
using DotBoil.Email.Configuration;
using DotBoil.Email.Models;
using DotBoil.EFCore;
using DotBoil.TemplateEngine;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using User = ButceYonet.Application.Domain.Entities.User;

namespace ButceYonet.Application.Infrastructure.Jobs;

public class SubscriptionReminderJob : BackgroundService
{
    private readonly ILogger<SubscriptionReminderJob> _logger;
    private readonly IServiceProvider _serviceProvider;

    public SubscriptionReminderJob(
        ILogger<SubscriptionReminderJob> logger,
        IServiceProvider serviceProvider)
    {
        _logger = logger;
        _serviceProvider = serviceProvider;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                await RunAsync(stoppingToken);
            }
            catch (OperationCanceledException)
            {
                _logger.LogInformation("Subscription reminder job timed out.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Subscription reminder job failed.");
            }

            await Task.Delay(TimeSpan.FromHours(24), stoppingToken);
        }
    }

    private async Task RunAsync(CancellationToken cancellationToken)
    {
        using var scope = _serviceProvider.CreateAsyncScope();
        var subscriptionRepository = scope.ServiceProvider.GetService<IRepository<Subscription, ButceYonetDbContext>>();
        var userRepository = scope.ServiceProvider.GetService<IRepository<User, ButceYonetAuthorizationDbContext>>();
        var currencyRepository = scope.ServiceProvider.GetService<IRepository<Currency, ButceYonetDbContext>>();
        var razorRenderer = scope.ServiceProvider.GetService<IRazorRenderer>();
        var mailSender = scope.ServiceProvider.GetService<IMailSender>();

        var today = DateTime.UtcNow.Date;
        var upcomingWindowEnd = today.AddDays(SubscriptionStatusCalculator.UpcomingWindowDays);

        var subscriptions = await subscriptionRepository
            .GetAll()
            .Where(s => s.NextOccurrence.HasValue && s.NextOccurrence.Value.Date <= upcomingWindowEnd)
            .ToListAsync(cancellationToken);

        if (subscriptions.Count == 0)
            return;

        var currencyMap = await currencyRepository.GetAll().ToDictionaryAsync(c => c.Id, cancellationToken);

        var overdueByUser = new Dictionary<int, List<Subscription>>();
        var upcomingByUser = new Dictionary<int, List<Subscription>>();

        foreach (var subscription in subscriptions)
        {
            var status = SubscriptionStatusCalculator.Calculate(subscription.NextOccurrence, subscription.LastPaidDate, today);

            if (status == SubscriptionStatus.Overdue)
                AddToBucket(overdueByUser, subscription);
            else if (status == SubscriptionStatus.Upcoming)
                AddToBucket(upcomingByUser, subscription);
        }

        var userIds = overdueByUser.Keys.Union(upcomingByUser.Keys).ToList();
        if (userIds.Count == 0)
            return;

        var serverSettings = DotBoilApp.Configuration.GetConfigurations<EmailOptions>();
        var serverSetting = serverSettings.ServerSettings.FirstOrDefault();

        foreach (var userId in userIds)
        {
            var user = await userRepository.Get()
                .Where(u => u.Id == userId)
                .FirstOrDefaultAsync(cancellationToken);
            if (user is null || string.IsNullOrWhiteSpace(user.Email))
                continue;

            var overdueItems = overdueByUser.TryGetValue(userId, out var overdueList)
                ? overdueList.Select(s => ToReminderItem(s, currencyMap, today)).ToList()
                : new List<SubscriptionReminderItem>();

            var upcomingItems = upcomingByUser.TryGetValue(userId, out var upcomingList)
                ? upcomingList.Select(s => ToReminderItem(s, currencyMap, today)).ToList()
                : new List<SubscriptionReminderItem>();

            var model = new SubscriptionReminderTemplateModel
            {
                UserName = string.IsNullOrWhiteSpace(user.Name) ? user.Email : $"{user.Name} {user.Surname}".Trim(),
                OverdueItems = overdueItems,
                UpcomingItems = upcomingItems,
                Year = DateTime.UtcNow.Year
            };

            try
            {
                var mailContent = await razorRenderer.RenderAsync("SubscriptionReminderTemplate", model);
                await mailSender.SendAsync(serverSetting.Value, new Message
                {
                    From = new List<string> { serverSetting.Value.EmailAddress },
                    To = new List<string> { user.Email },
                    Attachments = new List<Attachment>(),
                    Body = mailContent,
                    Subject = "Bütçe Yönet - Abonelik hatırlatması"
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Subscription reminder mail could not be sent for user {UserId}.", userId);
            }
        }
    }

    private static void AddToBucket(Dictionary<int, List<Subscription>> bucket, Subscription subscription)
    {
        if (!bucket.TryGetValue(subscription.UserId, out var list))
        {
            list = new List<Subscription>();
            bucket[subscription.UserId] = list;
        }

        list.Add(subscription);
    }

    private static SubscriptionReminderItem ToReminderItem(Subscription subscription, Dictionary<int, Currency> currencyMap, DateTime today)
    {
        var currency = subscription.CurrencyId.HasValue ? currencyMap.GetValueOrDefault(subscription.CurrencyId.Value) : null;

        return new SubscriptionReminderItem
        {
            Name = subscription.Name,
            Amount = subscription.Amount,
            CurrencySymbol = currency?.Symbol ?? "",
            IsSymbolRight = currency?.IsSymbolRight ?? true,
            DueDate = subscription.NextOccurrence!.Value,
            DaysDiff = (subscription.NextOccurrence.Value.Date - today).Days
        };
    }
}
