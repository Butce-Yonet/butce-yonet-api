using ButceYonet.Application.Domain.Entities;
using DotBoil.EFCore;
using DotBoil.EFCore.Attributes;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ButceYonet.Application.Infrastructure.Data.EntityTypeConfiguration;

[DotBoilEntityTypeConfiguration(typeof(ButceYonetDbContext))]
public class CategorizedTransactionReportV2EntityTypeConfiguration : EFCoreEntityTypeConfiguration<CategorizedTransactionReportV2>
{
    public override void ConfigureDotBoilEntity(EntityTypeBuilder<CategorizedTransactionReportV2> builder)
    {
        builder
            .Property(p => p.NotebookId)
            .IsRequired();

        builder
            .Property(p => p.UserLabelId)
            .IsRequired();

        builder
            .Property(p => p.TransactionType)
            .IsRequired();

        builder
            .Property(p => p.CurrencyId)
            .IsRequired();

        builder
            .Property(p => p.Amount)
            .IsRequired();

        builder
            .Property(p => p.Term)
            .IsRequired();

        builder
            .HasOne<Notebook>(p => p.Notebook)
            .WithMany()
            .HasForeignKey(p => p.NotebookId)
            .OnDelete(DeleteBehavior.NoAction);

        builder
            .HasOne<UserLabel>(p => p.UserLabel)
            .WithMany(p => p.CategorizedTransactionReportV2s)
            .HasForeignKey(p => p.UserLabelId)
            .OnDelete(DeleteBehavior.NoAction);

        builder
            .HasOne<Currency>(p => p.Currency)
            .WithMany()
            .HasForeignKey(p => p.CurrencyId)
            .OnDelete(DeleteBehavior.NoAction);

        builder
            .ToTable("CategorizedTransactionReportV2");
    }
}