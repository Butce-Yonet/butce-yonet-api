using ButceYonet.Application.Domain.Entities;
using ButceYonet.Application.Infrastructure.Configuration;
using DotBoil.EFCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace ButceYonet.Application.Infrastructure.Data;

public class ButceYonetDbContext : EFCoreDbContext
{
    public DbSet<Notebook> Notebooks { get; set; }
    public DbSet<NotebookUser> NotebookUsers { get; set; }
    public DbSet<Bank> Banks { get; set; }
    public DbSet<BankAccount> BankAccounts { get; set; }
    public DbSet<NotebookLabel> NotebookLabels { get; set; }
    public DbSet<Currency> Currencies { get; set; }
    public DbSet<Transaction> Transactions { get; set; }
    public DbSet<TransactionLabel> TransactionLabels { get; set; }
    public DbSet<Plan> Plans { get; set; }
    public DbSet<PlanFeature> PlanFeatures { get; set; }
    public DbSet<UserPlan> UserPlans { get; set; }
    
    public ButceYonetDbContext(IServiceProvider serviceProvider) : base(serviceProvider)
    {
    }

    protected override void ConfigureDatabaseProvider(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking);

        using var scope = _serviceProvider.CreateScope();
        var butceYonetConfiguration = scope.ServiceProvider.GetRequiredService<ButceYonetConfiguration>();
        
        optionsBuilder.UseMySQL(butceYonetConfiguration.MainConnectionString);
    }
}