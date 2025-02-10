using ButceYonet.Application.Domain.Entities;
using ButceYonet.Application.Infrastructure.Configuration;
using DotBoil.EFCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace ButceYonet.Application.Infrastructure.Data;

public class ButceYonetAuthorizationDbContext : EFCoreDbContext
{
    public DbSet<User> Users { get; set; }
    
    public ButceYonetAuthorizationDbContext(IServiceProvider serviceProvider) : base(serviceProvider)
    {
    }

    protected override void ConfigureDatabaseProvider(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking);
        
        using var scope = _serviceProvider.CreateScope();
        var butceYonetConfiguration = scope.ServiceProvider.GetRequiredService<ButceYonetConfiguration>();
        
        optionsBuilder.UseMySQL(butceYonetConfiguration.AuthorizationConnectionString);
    }
}