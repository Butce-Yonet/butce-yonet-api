using ButceYonet.Application.Infrastructure;
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
        return Task.CompletedTask;
    }

    public override Task UseModule()
    {
        return Task.CompletedTask;
    }
}