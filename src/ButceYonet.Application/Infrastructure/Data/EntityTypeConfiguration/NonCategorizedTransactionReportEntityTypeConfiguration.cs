using ButceYonet.Application.Domain.Entities;
using DotBoil.EFCore;
using DotBoil.EFCore.Attributes;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ButceYonet.Application.Infrastructure.Data.EntityTypeConfiguration;

[DotBoilEntityTypeConfiguration(typeof(ButceYonetDbContext))]
public class NonCategorizedTransactionReportEntityTypeConfiguration : EFCoreEntityTypeConfiguration<NonCategorizedTransactionReport>
{
    public override void ConfigureDotBoilEntity(EntityTypeBuilder<NonCategorizedTransactionReport> builder)
    {
        builder
            .Property(p => p.NotebookId)
            .IsRequired();

        builder
            .Property(p => p.TransactionType)
            .IsRequired();

        builder
            .Property(p => p.Amount)
            .IsRequired();

        builder
            .Property(p => p.Term)
            .IsRequired();
        
        builder
            .HasOne<Notebook>(p => p.Notebook)
            .WithMany(p => p.NonCategorizedTransactionReports)
            .HasForeignKey(p => p.NotebookId)
            .OnDelete(DeleteBehavior.NoAction);
        
        builder
            .HasOne<Currency>(p => p.Currency)
            .WithMany(p => p.NonCategorizedTransactionReports)
            .HasForeignKey(p => p.CurrencyId)
            .OnDelete(DeleteBehavior.NoAction);
        
        builder
            .ToTable("NonCategorizedTransactionReport");
    }
}