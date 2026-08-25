using ButceYonet.Application.Domain.Entities;
using DotBoil.EFCore;
using DotBoil.EFCore.Attributes;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ButceYonet.Application.Infrastructure.Data.EntityTypeConfiguration;

[DotBoilEntityTypeConfiguration(typeof(ButceYonetDbContext))]
public class GoalLabelEntityTypeConfiguration : EFCoreEntityTypeConfiguration<GoalLabel>
{
    public override void ConfigureDotBoilEntity(EntityTypeBuilder<GoalLabel> builder)
    {
        builder
            .Property(p => p.GoalId)
            .IsRequired();

        builder
            .Property(p => p.UserLabelId)
            .IsRequired();

        builder
            .HasOne(p => p.UserLabel)
            .WithMany(p => p.GoalLabels)
            .HasForeignKey(p => p.UserLabelId)
            .OnDelete(DeleteBehavior.NoAction);

        builder
            .HasOne(p => p.Goal)
            .WithMany(p => p.GoalLabels)
            .HasForeignKey(p => p.GoalId)
            .OnDelete(DeleteBehavior.NoAction);

        builder
            .ToTable("GoalLabels");
    }
}
