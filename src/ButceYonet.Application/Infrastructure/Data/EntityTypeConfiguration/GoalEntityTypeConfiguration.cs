using ButceYonet.Application.Domain.Entities;
using DotBoil.EFCore;
using DotBoil.EFCore.Attributes;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ButceYonet.Application.Infrastructure.Data.EntityTypeConfiguration;

[DotBoilEntityTypeConfiguration(typeof(ButceYonetDbContext))]
public class GoalEntityTypeConfiguration : EFCoreEntityTypeConfiguration<Goal>
{
    public override void ConfigureDotBoilEntity(EntityTypeBuilder<Goal> builder)
    {
        builder
            .Property(p => p.UserId)
            .IsRequired();

        builder
            .Property(p => p.Name)
            .IsRequired()
            .HasMaxLength(128);

        builder
            .Property(p => p.TargetAmount)
            .IsRequired();

        builder
            .Property(p => p.CurrentAmount)
            .IsRequired();

        builder
            .Property(p => p.CurrencyId)
            .IsRequired();

        builder
            .Property(p => p.Deadline);

        builder
            .HasOne(p => p.Currency)
            .WithMany()
            .HasForeignKey(p => p.CurrencyId)
            .OnDelete(DeleteBehavior.NoAction);

        builder
            .ToTable("Goals");
    }
}
