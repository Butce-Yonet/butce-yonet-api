using ButceYonet.Application.Application;
using ButceYonet.Application.Application.Interfaces;
using ButceYonet.Application.Application.Shared.UserPlanRuleValidators;
using ButceYonet.Application.Domain.Enums;
using ButceYonet.Application.Infrastructure;
using ButceYonet.Application.Infrastructure.Services;
using DotBoil;
using DotBoil.Dependency;
using DotBoil.EFCore;
using DotBoil.Localization;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace ButceYonet.Application;

public class ButceYonetModule : Module
{
    public override Task AddModule()
    {
        DotBoilApp.Services.AddScoped<IAuditUser, CurrentUser>();
        DotBoilApp.Services.AddScoped<ICurrentLanguage, CurrentLanguage>();
        DotBoilApp.Services.AddScoped<IUserPlanValidator, UserPlanValidator>();
        DotBoilApp.Services.AddScoped<IUser, User>();
        DotBoilApp.Services.AddScoped<IRecurringTransactionIntervalsService, RecurringTransactionIntervalsService>();
        DotBoilApp.Services.AddScoped<JwtAuthFilterAttribute>();

        #region User Plan Rule Validators

        DotBoilApp.Services.AddKeyedScoped<IUserPlanRuleValidator, NotebookCountRuleValidator>(PlanFeatures.NotebookCount
            .ToString());
        
        DotBoilApp.Services.AddKeyedScoped<IUserPlanRuleValidator, NotebookUserCountRuleValidator>(PlanFeatures.NotebookUserCount
            .ToString());
        
        DotBoilApp.Services.AddKeyedScoped<IUserPlanRuleValidator, NotebookLabelCountRuleValidator>(PlanFeatures.NotebookLabelCount
            .ToString());
        
        DotBoilApp.Services.AddKeyedScoped<IUserPlanRuleValidator, NotebookTransactionCount>(PlanFeatures.NotebookTransactionCount
            .ToString());
        
        DotBoilApp.Services.AddKeyedScoped<IUserPlanRuleValidator, BankAccountCountRuleValidator>(PlanFeatures.BankAccountCount
            .ToString());

        #endregion
        
        return Task.CompletedTask;
    }

    public override Task UseModule()
    {
        ((WebApplication)DotBoilApp.Host).UseMiddleware<ApiExceptionHandlingMiddleware>();
        return Task.CompletedTask;
    }
}