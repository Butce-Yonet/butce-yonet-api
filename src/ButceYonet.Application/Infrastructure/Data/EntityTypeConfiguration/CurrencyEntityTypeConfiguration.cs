using ButceYonet.Application.Domain.Entities;
using DotBoil.EFCore;
using DotBoil.EFCore.Attributes;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ButceYonet.Application.Infrastructure.Data.EntityTypeConfiguration;

[DotBoilEntityTypeConfiguration(typeof(ButceYonetDbContext))]
public class CurrencyEntityTypeConfiguration : EFCoreEntityTypeConfiguration<Currency>
{
    public override void ConfigureDotBoilEntity(EntityTypeBuilder<Currency> builder)
    {
        builder
            .Property(p => p.Code)
            .HasMaxLength(64)
            .IsRequired();

        builder
            .Property(p => p.Name)
            .HasMaxLength(64)
            .IsRequired();

        builder
            .Property(p => p.Symbol)
            .HasMaxLength(10)
            .IsRequired();

        builder
            .Property(p => p.IsSymbolRight)
            .IsRequired();

        builder
            .HasMany<Transaction>(p => p.Transactions)
            .WithOne(p => p.Currency)
            .HasForeignKey(p => p.CurrencyId)
            .OnDelete(DeleteBehavior.NoAction);
        
        builder
            .HasMany<CategorizedTransactionReport>(p => p.CategorizedTransactionReports)
            .WithOne(p => p.Currency)
            .HasForeignKey(p => p.CurrencyId)
            .OnDelete(DeleteBehavior.NoAction);
        
        builder
            .HasMany<NonCategorizedTransactionReport>(p => p.NonCategorizedTransactionReports)
            .WithOne(p => p.Currency)
            .HasForeignKey(p => p.CurrencyId)
            .OnDelete(DeleteBehavior.NoAction);
        
        builder
            .ToTable("Currencies");
    }
}