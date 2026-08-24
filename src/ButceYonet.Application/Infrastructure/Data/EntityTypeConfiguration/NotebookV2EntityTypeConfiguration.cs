using ButceYonet.Application.Domain.Entities;
using DotBoil.EFCore;
using DotBoil.EFCore.Attributes;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ButceYonet.Application.Infrastructure.Data.EntityTypeConfiguration;

[DotBoilEntityTypeConfiguration(typeof(ButceYonetDbContext))]
public class NotebookV2EntityTypeConfiguration : EFCoreEntityTypeConfiguration<NotebookV2>
{
    public override void ConfigureDotBoilEntity(EntityTypeBuilder<NotebookV2> builder)
    {
        builder
            .Property(p => p.UserId)
            .IsRequired();

        builder
            .Property(p => p.Name)
            .IsRequired()
            .HasMaxLength(50);

        builder
            .Property(p => p.TermStart)
            .IsRequired();

        builder
            .Property(p => p.TermEnd)
            .IsRequired();

        builder
            .HasIndex(p => p.UserId);

        builder
            .HasIndex(p => new { p.UserId, p.TermStart })
            .IsUnique();

        builder
            .HasMany<TransactionV2>(p => p.Transactions)
            .WithOne(p => p.NotebookV2)
            .HasForeignKey(p => p.NotebookV2Id)
            .OnDelete(DeleteBehavior.NoAction);

        builder
            .HasMany<NonCategorizedTransactionReport>(p => p.NonCategorizedTransactionReports)
            .WithOne(p => p.NotebookV2)
            .HasForeignKey(p => p.NotebookV2Id)
            .OnDelete(DeleteBehavior.NoAction);

        builder
            .ToTable("NotebookV2");
    }
}
