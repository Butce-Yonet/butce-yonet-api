using ButceYonet.Application.Domain.Entities;
using DotBoil.EFCore;
using DotBoil.EFCore.Attributes;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ButceYonet.Application.Infrastructure.Data.EntityTypeConfiguration;

[DotBoilEntityTypeConfiguration(typeof(ButceYonetDbContext))]
public class CategorizedTransactionReportEntityTypeConfiguration : EFCoreEntityTypeConfiguration<CategorizedTransactionReport>
{
    public override void ConfigureDotBoilEntity(EntityTypeBuilder<CategorizedTransactionReport> builder)
    {
        builder
            .Property(p => p.NotebookId)
            .IsRequired();

        builder
            .Property(p => p.NotebookLabelId)
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
            .HasOne<Notebook>()
            .WithMany(p => p.CategorizedTransactionReports)
            .HasForeignKey(p => p.NotebookId)
            .OnDelete(DeleteBehavior.NoAction);

        builder
            .HasOne<NotebookLabel>()
            .WithMany(p => p.CategorizedTransactionReports)
            .HasForeignKey(p => p.NotebookLabelId)
            .OnDelete(DeleteBehavior.NoAction);

        builder
            .HasOne<Currency>()
            .WithMany(p => p.CategorizedTransactionReports)
            .HasForeignKey(p => p.CurrencyId)
            .OnDelete(DeleteBehavior.NoAction);
        
        builder
            .ToTable("CategorizedTransactionReport");
    }
}