using ButceYonet.Application.Domain.Entities;
using DotBoil.EFCore;
using DotBoil.EFCore.Attributes;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ButceYonet.Application.Infrastructure.Data.EntityTypeConfiguration;

[DotBoilEntityTypeConfiguration(typeof(ButceYonetDbContext))]
public class UserPlanEntityTypeConfiguration : EFCoreEntityTypeConfiguration<UserPlan>
{
    public override void ConfigureDotBoilEntity(EntityTypeBuilder<UserPlan> builder)
    {
        builder
            .Property(p => p.UserId)
            .IsRequired();

        builder
            .Property(p => p.PlanId)
            .IsRequired();

        builder
            .Property(p => p.ExpirationDate);

        builder
            .HasOne<Plan>(p => p.Plan)
            .WithMany(p => p.UserPlans)
            .HasForeignKey(p => p.PlanId)
            .OnDelete(DeleteBehavior.NoAction);
        
        builder
            .ToTable("UserPlans");
    }
}