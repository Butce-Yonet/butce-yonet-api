using ButceYonet.Application.Domain.Entities;
using DotBoil.EFCore;
using DotBoil.EFCore.Attributes;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ButceYonet.Application.Infrastructure.Data.EntityTypeConfiguration;

[DotBoilEntityTypeConfiguration(typeof(ButceYonetDbContext))]
public class SubscriptionEntityTypeConfiguration : EFCoreEntityTypeConfiguration<Subscription>
{
    public override void ConfigureDotBoilEntity(EntityTypeBuilder<Subscription> builder)
    {
        builder
            .Property(p => p.UserId)
            .IsRequired();

        builder
            .Property(p => p.Name)
            .IsRequired()
            .HasMaxLength(128);

        builder
            .Property(p => p.StartDate)
            .IsRequired();

        builder
            .Property(p => p.Frequency)
            .IsRequired();

        builder
            .Property(p => p.Interval);

        builder
            .Property(p => p.NextOccurrence);

        builder
            .Property(p => p.LastPaidDate);

        builder
            .HasOne(p => p.Currency)
            .WithMany()
            .HasForeignKey(p => p.CurrencyId)
            .OnDelete(DeleteBehavior.NoAction);

        builder
            .ToTable("Subscriptions");
    }
}
