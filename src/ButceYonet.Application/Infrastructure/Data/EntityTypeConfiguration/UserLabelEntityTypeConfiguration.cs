using ButceYonet.Application.Domain.Entities;
using DotBoil.EFCore;
using DotBoil.EFCore.Attributes;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ButceYonet.Application.Infrastructure.Data.EntityTypeConfiguration;

[DotBoilEntityTypeConfiguration(typeof(ButceYonetDbContext))]
public class UserLabelEntityTypeConfiguration : EFCoreEntityTypeConfiguration<UserLabel>
{
    public override void ConfigureDotBoilEntity(EntityTypeBuilder<UserLabel> builder)
    {
        builder
            .Property(p => p.UserId);

        builder
            .Property(p => p.Name)
            .HasMaxLength(128)
            .IsRequired();

        builder
            .Property(p => p.ColorCode)
            .HasMaxLength(16)
            .IsRequired();

        builder
            .HasMany<TransactionLabelV2>(p => p.TransactionLabelsV2)
            .WithOne(p => p.UserLabel)
            .HasForeignKey(p => p.UserLabelId)
            .OnDelete(DeleteBehavior.NoAction);

        builder
            .HasMany<CategorizedTransactionReportV2>(p => p.CategorizedTransactionReportV2s)
            .WithOne(p => p.UserLabel)
            .HasForeignKey(p => p.UserLabelId)
            .OnDelete(DeleteBehavior.NoAction);

        builder
            .ToTable("UserLabels");
    }
}