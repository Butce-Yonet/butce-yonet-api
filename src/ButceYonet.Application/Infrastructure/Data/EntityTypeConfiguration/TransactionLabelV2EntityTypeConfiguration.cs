using ButceYonet.Application.Domain.Entities;
using DotBoil.EFCore;
using DotBoil.EFCore.Attributes;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ButceYonet.Application.Infrastructure.Data.EntityTypeConfiguration;

[DotBoilEntityTypeConfiguration(typeof(ButceYonetDbContext))]
public class TransactionLabelV2EntityTypeConfiguration : EFCoreEntityTypeConfiguration<TransactionLabelV2>
{
    public override void ConfigureDotBoilEntity(EntityTypeBuilder<TransactionLabelV2> builder)
    {
        builder
            .Property(p => p.UserLabelId)
            .IsRequired();

        builder
            .Property(p => p.TransactionV2Id)
            .IsRequired();

        builder
            .HasOne<UserLabel>(p => p.UserLabel)
            .WithMany(p => p.TransactionLabelsV2)
            .HasForeignKey(p => p.UserLabelId)
            .OnDelete(DeleteBehavior.NoAction);

        builder
            .HasOne<TransactionV2>(p => p.TransactionV2)
            .WithMany(p => p.TransactionLabelsV2)
            .HasForeignKey(p => p.TransactionV2Id)
            .OnDelete(DeleteBehavior.NoAction);

        builder
            .ToTable("TransactionLabelsV2");
    }
}