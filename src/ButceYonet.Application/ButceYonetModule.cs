using ButceYonet.Application.Application;
using ButceYonet.Application.Application.Interfaces;
using ButceYonet.Application.Application.Shared.Profiles;
using ButceYonet.Application.Application.Shared.UserPlanRuleValidators;
using ButceYonet.Application.Domain.Enums;
using ButceYonet.Application.Infrastructure;
using ButceYonet.Application.Infrastructure.Configuration;
using ButceYonet.Application.Infrastructure.Jobs;
using ButceYonet.Application.Infrastructure.Notifications;
using ButceYonet.Application.Infrastructure.Services;
using DotBoil;
using DotBoil.Configuration;
using DotBoil.Dependency;
using DotBoil.EFCore;
using DotBoil.Localization;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace ButceYonet.Application;

public class ButceYonetModule : Module
{
    public override string Name { get; } = "ButceYonetModule";

    public override IEnumerable<string> DependsOn { get; } = new List<string>()
    {
    };

    public override int Order { get; } = 99;
    
    public override Task AddModule()
    {
        var configuration = DotBoilApp.Configuration.GetConfigurations<ButceYonetConfiguration>();

        DotBoilApp.Services.AddSingleton(typeof(ButceYonetConfiguration), configuration);
        DotBoilApp.Services.AddScoped<IAuditUser, CurrentUser>();
        DotBoilApp.Services.AddScoped<ICurrentLanguage, CurrentLanguage>();
        DotBoilApp.Services.AddScoped<IUserPlanValidator, UserPlanValidator>();
        DotBoilApp.Services.AddScoped<IUser, User>();
        DotBoilApp.Services.AddScoped<IRecurringTransactionIntervalsService, RecurringTransactionIntervalsService>();
        DotBoilApp.Services.AddScoped<INotebookPeriodResolver, NotebookPeriodResolver>();
        DotBoilApp.Services.AddScoped<ITransactionReportSyncService, TransactionReportSyncService>();

        DotBoilApp.Services.AddSingleton<INotificationLocalizer, NotificationLocalizer>();
        DotBoilApp.Services.AddScoped<INotificationRecipientProvider, NotificationRecipientProvider>();
        DotBoilApp.Services.AddScoped<IMailNotificationSender, MailNotificationSender>();
        DotBoilApp.Services.AddScoped<IPushNotificationSender, PushNotificationSender>();

        RegisterHostedServiceIfEnabled<RecurringTransactionJob>(configuration);
        RegisterHostedServiceIfEnabled<SubscriptionReminderJob>(configuration);
        RegisterHostedServiceIfEnabled<RecalculateNonCategorizedTransactionReport>(configuration);
        RegisterHostedServiceIfEnabled<RecalculateCategorizedTransactionReport>(configuration);
        RegisterHostedServiceIfEnabled<EngagementSignalJob>(configuration);
        RegisterHostedServiceIfEnabled<EngagementSignalDispatchJob>(configuration);
        #region User Plan Rule Validators

        DotBoilApp.Services.AddKeyedScoped<IUserPlanRuleValidator, NotebookTransactionCount>(PlanFeatures.NotebookTransactionCount
            .ToString());

        #endregion

        DotBoilApp.Services.AddTransient<RecurringTransactionProfile.TransactionResolver>();

        DotBoilApp.Services.AddTransient<NonCategorizedTransactionReportProfile.NotebookResolver>();
        DotBoilApp.Services.AddTransient<NonCategorizedTransactionReportProfile.CurrencyResolver>();
        
        return Task.CompletedTask;
    }

    public override Task UseModule()
    {
        ((WebApplication)DotBoilApp.Host).UseMiddleware<ApiExceptionHandlingMiddleware>();
        return Task.CompletedTask;
    }

    private static void RegisterHostedServiceIfEnabled<THostedService>(ButceYonetConfiguration configuration)
        where THostedService : class, IHostedService
    {
        if (configuration.Jobs.Contains(typeof(THostedService).Name, StringComparer.OrdinalIgnoreCase))
        {
            DotBoilApp.Services.AddHostedService<THostedService>();
        }
    }
}