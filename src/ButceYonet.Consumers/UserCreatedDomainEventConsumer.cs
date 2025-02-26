using ButceYonet.Application.Domain.Constants;
using ButceYonet.Application.Domain.Entities;
using ButceYonet.Application.Domain.Events;
using ButceYonet.Application.Infrastructure.Data;
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
    private IRepository<Notebook, ButceYonetDbContext> _notebookRepository;
    private ICache _cache;
    
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
        await InitializeNotebook(context.Message);
    }

    private void InitializeDependencies(IServiceScope scope)
    {
        _userRepository =
            scope.ServiceProvider.GetRequiredService<IRepository<User, ButceYonetAuthorizationDbContext>>();
        _planRepository = scope.ServiceProvider.GetRequiredService<IRepository<Plan, ButceYonetDbContext>>();
        _userPlanRepository = scope.ServiceProvider.GetRequiredService<IRepository<UserPlan, ButceYonetDbContext>>();
        _notebookRepository = scope.ServiceProvider.GetRequiredService<IRepository<Notebook, ButceYonetDbContext>>();
        _cache = scope.ServiceProvider.GetRequiredService<ICache>();
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

    private async Task InitializeNotebook(UserCreatedDomainEvent @event)
    {
        var user = await _userRepository.Get().Where(p => p.Email == @event.Email).FirstOrDefaultAsync();

        if (user is null)
            return;
        
        var notebook = new Notebook()
        {
            Name = "İlk defterim",
            IsDefault = true
        };

        var notebookUser = new NotebookUser()
        {
            UserId = @user.Id,
            IsDefault = true
        };
        
        notebook.NotebookUsers.Add(notebookUser);

        var notebookCreatedDomainEvent = new NotebookCreatedDomainEvent(notebook);
        
        notebook.AddEvent(notebookCreatedDomainEvent);
        
        await _notebookRepository.AddAsync(notebook);
        await _notebookRepository.SaveChangesAsync();
    }
}