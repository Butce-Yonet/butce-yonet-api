using ButceYonet.Application.Domain.Entities;
using DotBoil.EFCore;
using DotBoil.EFCore.Attributes;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ButceYonet.Application.Infrastructure.Data.EntityTypeConfiguration;

[DotBoilEntityTypeConfiguration(typeof(PlanFeature))]
public class PlanFeatureEntityTypeConfiguration : EFCoreEntityTypeConfiguration<PlanFeature>
{
    public override void ConfigureDotBoilEntity(EntityTypeBuilder<PlanFeature> builder)
    {
        builder
            .Property(p => p.PlanId)
            .IsRequired();

        builder
            .Property(p => p.Feature)
            .IsRequired();

        builder
            .Property(p => p.Count)
            .IsRequired();

        builder
            .Property(p => p.Description)
            .HasMaxLength(128)
            .IsRequired();

        builder
            .HasOne<Plan>(p => p.Plan)
            .WithMany(p => p.PlanFeatures)
            .HasForeignKey(p => p.PlanId)
            .IsRequired();
        
        builder
            .ToTable("PlanFeatures");
    }
}