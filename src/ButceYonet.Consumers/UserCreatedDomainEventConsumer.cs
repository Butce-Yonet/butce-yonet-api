using ButceYonet.Application.Domain.Constants;
using ButceYonet.Application.Domain.Entities;
using ButceYonet.Application.Domain.Events;
using ButceYonet.Application.Infrastructure.Data;
using ButceYonet.Application.Infrastructure.MailTemplates;
using DotBoil;
using DotBoil.AuthGuard.Application.Domain.DomainEvents;
using DotBoil.Caching;
using DotBoil.Configuration;
using DotBoil.EFCore;
using DotBoil.Email;
using DotBoil.Email.Configuration;
using DotBoil.Email.Models;
using DotBoil.MassTransit.Attributes;
using DotBoil.MassTransit.Consumers;
using DotBoil.TemplateEngine;
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
    private IRazorRenderer _razorRenderer;
    private IMailSender _mailSender;
    
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
        await SendWelcomeMail(context.Message);
    }

    private void InitializeDependencies(IServiceScope scope)
    {
        _userRepository =
            scope.ServiceProvider.GetRequiredService<IRepository<User, ButceYonetAuthorizationDbContext>>();
        _planRepository = scope.ServiceProvider.GetRequiredService<IRepository<Plan, ButceYonetDbContext>>();
        _userPlanRepository = scope.ServiceProvider.GetRequiredService<IRepository<UserPlan, ButceYonetDbContext>>();
        _notebookRepository = scope.ServiceProvider.GetRequiredService<IRepository<Notebook, ButceYonetDbContext>>();
        _cache = scope.ServiceProvider.GetRequiredService<ICache>();
        _razorRenderer = scope.ServiceProvider.GetRequiredService<IRazorRenderer>();
        _mailSender = scope.ServiceProvider.GetRequiredService<IMailSender>();
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

    private async Task SendWelcomeMail(UserCreatedDomainEvent @event)
    {
        var userCreatedMailTemplateModel = new UserCreatedMailTemplateModel
        {
            UserName = string.Format("{0} {1}", @event.Name, @event.Surname),
            Year = DateTime.Now.Year
        };

        var mailContent = await _razorRenderer.RenderAsync("UserCreatedMailTemplate", userCreatedMailTemplateModel);
        var mailConfiguration = DotBoilApp.Configuration.GetConfigurations<EmailOptions>();
        var serverSettings = mailConfiguration.ServerSettings;
        var serverSetting = serverSettings.FirstOrDefault();
        
        await _mailSender.SendAsync(serverSetting.Value, new Message()
        {
            From = new List<string>() { serverSetting.Value.EmailAddress },
            To = new List<string>() { @event.Email },
            Attachments = new List<Attachment>(),
            Body = mailContent,
            Subject = "Bütçe Yönet"
        });

    }
}