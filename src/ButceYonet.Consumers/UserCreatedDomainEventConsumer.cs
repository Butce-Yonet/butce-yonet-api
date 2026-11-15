using ButceYonet.Application.Domain.Constants;
using ButceYonet.Application.Domain.Entities;
using ButceYonet.Application.Domain.Events;
using ButceYonet.Application.Infrastructure.Data;
using ButceYonet.Application.Infrastructure.MailTemplates;
using ButceYonet.Application.Infrastructure.Notifications;
using DotBoil.AuthGuard.Application.Domain.DomainEvents;
using DotBoil.Caching;
using DotBoil.EFCore;
using DotBoil.MassTransit.Attributes;
using DotBoil.MassTransit.Consumers;
using MassTransit;
using Microsoft.EntityFrameworkCore;

namespace ButceYonet.Consumers;

[Consumer("user-created")]
public class UserCreatedDomainEventConsumer : BaseConsumer<UserCreatedDomainEvent>
{
    private IRepository<User, ButceYonetAuthorizationDbContext> _userRepository;
    private IRepository<Plan, ButceYonetDbContext> _planRepository;
    private IRepository<UserPlan, ButceYonetDbContext> _userPlanRepository;
    private IRepository<DefaultLabel, ButceYonetDbContext> _defaultLabelRepository;
    private IRepository<UserLabel, ButceYonetDbContext> _userLabelRepository;
    private ICache _cache;
    private INotificationRecipientProvider _recipientProvider;
    private IMailNotificationSender _mailSender;
    
    private readonly IServiceProvider _serviceProvider;
    
    public UserCreatedDomainEventConsumer(IServiceProvider serviceProvider) : base(serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public override async Task ConsumeEvent(ConsumeContext<UserCreatedDomainEvent> context)
    {
        using var scope = _serviceProvider.CreateScope();
        InitializeDependencies(scope);
        await InitializeUserPlan(context.Message);
        await InitializeUserLabels(context.Message);
        await SendWelcomeMail(context.Message);
    }

    private void InitializeDependencies(IServiceScope scope)
    {
        _userRepository =
            scope.ServiceProvider.GetRequiredService<IRepository<User, ButceYonetAuthorizationDbContext>>();
        _planRepository = scope.ServiceProvider.GetRequiredService<IRepository<Plan, ButceYonetDbContext>>();
        _userPlanRepository = scope.ServiceProvider.GetRequiredService<IRepository<UserPlan, ButceYonetDbContext>>();
        _defaultLabelRepository = scope.ServiceProvider.GetRequiredService<IRepository<DefaultLabel, ButceYonetDbContext>>();
        _userLabelRepository = scope.ServiceProvider.GetRequiredService<IRepository<UserLabel, ButceYonetDbContext>>();
        _cache = scope.ServiceProvider.GetRequiredService<ICache>();
        _recipientProvider = scope.ServiceProvider.GetRequiredService<INotificationRecipientProvider>();
        _mailSender = scope.ServiceProvider.GetRequiredService<IMailNotificationSender>();
    }

    private async Task InitializeUserPlan(UserCreatedDomainEvent @event)
    {
        var defaultPlan = await _cache.GetOrSetAsync(CacheKeyConstants.DefaultUserPlan, async () =>
        {
            return await _planRepository.Get().Where(p => p.IsDefault).FirstOrDefaultAsync();
        }, CacheIntervalConstants.DefaultUserPlan);

        if (defaultPlan is null)
            return;

        var user = await _userRepository.Get().Where(p => p.Email == @event.Email).FirstOrDefaultAsync();

        if (user is null)
            return;

        var userPlan = new UserPlan
        {
            UserId = user.Id,
            PlanId = defaultPlan.Id,
            ExpirationDate = null
        };

        var userPlanCreatedDomainEvent = new UserPlanCreatedDomainEvent(user.Id, defaultPlan.Id);
        userPlan.AddEvent(userPlanCreatedDomainEvent);

        await _userPlanRepository.AddAsync(userPlan);
        await _userRepository.SaveChangesAsync();
    }

    private async Task InitializeUserLabels(UserCreatedDomainEvent @event)
    {
        var defaultLabels = await _cache.GetOrSetAsync(CacheKeyConstants.DefaultNotebookLabels, async () =>
        {
            return await _defaultLabelRepository.GetAll().ToListAsync();
        }, CacheIntervalConstants.DefaultNotebookLabels);

        if (!defaultLabels.Any())
            return;

        var user = await _userRepository.Get().Where(p => p.Email == @event.Email).FirstOrDefaultAsync();

        if (user is null)
            return;

        var userLabels = defaultLabels.Select(dl => new UserLabel
        {
            UserId = user.Id,
            Name = dl.Name,
            ColorCode = dl.ColorCode
        }).ToList();

        await _userLabelRepository.AddRangeAsync(userLabels);
        await _userLabelRepository.SaveChangesAsync();
    }

    private async Task SendWelcomeMail(UserCreatedDomainEvent @event)
    {
        var recipient = await _recipientProvider.GetByEmailAsync(@event.Email)
                        ?? new NotificationRecipient { Email = @event.Email };

        var userCreatedMailTemplateModel = new UserCreatedMailTemplateModel
        {
            UserName = string.Format("{0} {1}", @event.Name, @event.Surname)
        };

        // Hoş geldin maili işlemsel: kayıt anında tercih satırı henüz olamayacağı için Email tercihine bakılmaz.
        await _mailSender.SendAsync(recipient, "UserCreatedMailTemplate", "UserCreated.Subject", userCreatedMailTemplateModel,
            ignorePreferences: true);
    }
}