using ButceYonet.Application.Application.Interfaces;
using ButceYonet.Application.Infrastructure;
using ButceYonet.Application.Infrastructure.Services;
using DotBoil;
using DotBoil.Dependency;
using DotBoil.EFCore;
using DotBoil.Localization;
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
        return Task.CompletedTask;
    }

    public override Task UseModule()
    {
        return Task.CompletedTask;
    }
}