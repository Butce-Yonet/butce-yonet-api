using ButceYonet.Application.Domain.Entities;
using DotBoil.EFCore;
using DotBoil.EFCore.Attributes;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Mysqlx.Crud;

namespace ButceYonet.Application.Infrastructure.Data.EntityTypeConfiguration;

[DotBoilEntityTypeConfiguration(typeof(ButceYonetDbContext))]
public class PlanEntityTypeConfiguration : EFCoreEntityTypeConfiguration<Plan>
{
    public override void ConfigureDotBoilEntity(EntityTypeBuilder<Plan> builder)
    {
        builder
            .Property(p => p.Title)
            .HasMaxLength(128)
            .IsRequired();

        builder
            .Property(p => p.Description)
            .HasMaxLength(256)
            .IsRequired();

        builder
            .Property(p => p.Amount)
            .IsRequired();

        builder
            .Property(p => p.IsDefault)
            .IsRequired();

        builder
            .HasMany<PlanFeature>(p => p.PlanFeatures)
            .WithOne(p => p.Plan)
            .HasForeignKey(p => p.PlanId)
            .OnDelete(DeleteBehavior.NoAction);

        builder
            .HasMany<UserPlan>(p => p.UserPlans)
            .WithOne(p => p.Plan)
            .HasForeignKey(p => p.PlanId)
            .OnDelete(DeleteBehavior.NoAction);
        
        builder
            .ToTable("Plans");
    }
}