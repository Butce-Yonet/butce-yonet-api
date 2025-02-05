using DotBoil.EFCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace ButceYonet.Application.Infrastructure.Data;

public class DbContextLoader : EFCoreDbContextLoader
{
    public override Task LoadDbContext(IConfiguration configuration, IServiceCollection services)
    {
        services.AddDbContext<ButceYonetDbContext>();
        return Task.CompletedTask;
    }
}