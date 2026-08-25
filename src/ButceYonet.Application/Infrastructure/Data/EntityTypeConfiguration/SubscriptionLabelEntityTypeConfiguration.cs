using ButceYonet.Application.Domain.Entities;
using DotBoil.EFCore;
using DotBoil.EFCore.Attributes;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ButceYonet.Application.Infrastructure.Data.EntityTypeConfiguration;

[DotBoilEntityTypeConfiguration(typeof(ButceYonetDbContext))]
public class SubscriptionLabelEntityTypeConfiguration : EFCoreEntityTypeConfiguration<SubscriptionLabel>
{
    public override void ConfigureDotBoilEntity(EntityTypeBuilder<SubscriptionLabel> builder)
    {
        builder
            .Property(p => p.SubscriptionId)
            .IsRequired();

        builder
            .Property(p => p.UserLabelId)
            .IsRequired();

        builder
            .HasOne(p => p.UserLabel)
            .WithMany(p => p.SubscriptionLabels)
            .HasForeignKey(p => p.UserLabelId)
            .OnDelete(DeleteBehavior.NoAction);

        builder
            .HasOne(p => p.Subscription)
            .WithMany(p => p.SubscriptionLabels)
            .HasForeignKey(p => p.SubscriptionId)
            .OnDelete(DeleteBehavior.NoAction);

        builder
            .ToTable("SubscriptionLabels");
    }
}
