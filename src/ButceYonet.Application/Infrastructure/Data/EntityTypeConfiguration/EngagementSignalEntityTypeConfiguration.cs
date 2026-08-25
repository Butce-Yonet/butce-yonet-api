using ButceYonet.Application.Domain.Entities;
using DotBoil.EFCore;
using DotBoil.EFCore.Attributes;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ButceYonet.Application.Infrastructure.Data.EntityTypeConfiguration;

[DotBoilEntityTypeConfiguration(typeof(ButceYonetDbContext))]
public class EngagementSignalEntityTypeConfiguration : EFCoreEntityTypeConfiguration<EngagementSignal>
{
    public override void ConfigureDotBoilEntity(EntityTypeBuilder<EngagementSignal> builder)
    {
        builder
            .Property(p => p.UserId)
            .IsRequired();

        builder
            .Property(p => p.Type)
            .IsRequired();

        builder
            .Property(p => p.OccurredAt)
            .IsRequired();

        builder
            .Property(p => p.PayloadJson)
            .IsRequired();

        builder
            .Property(p => p.IsSent)
            .IsRequired()
            .HasDefaultValue(false);

        builder
            .HasOne(p => p.Goal)
            .WithMany()
            .HasForeignKey(p => p.GoalId)
            .OnDelete(DeleteBehavior.NoAction);

        builder
            .HasOne(p => p.Transaction)
            .WithMany()
            .HasForeignKey(p => p.TransactionId)
            .OnDelete(DeleteBehavior.NoAction);

        builder
            .HasIndex(p => p.UserId);

        builder
            .HasIndex(p => p.OccurredAt);

        // Faz II'nin "gönderilmemiş sinyalleri sırayla çek" sorgusunu (WHERE IsSent = false ORDER BY OccurredAt) destekler.
        builder
            .HasIndex(p => new { p.IsSent, p.OccurredAt });

        builder
            .ToTable("EngagementSignals");
    }
}
