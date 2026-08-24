using ButceYonet.Application.Domain.Entities;
using DotBoil.EFCore;
using DotBoil.EFCore.Attributes;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ButceYonet.Application.Infrastructure.Data.EntityTypeConfiguration;

[DotBoilEntityTypeConfiguration(typeof(ButceYonetDbContext))]
public class TransactionV2EntityTypeConfiguration : EFCoreEntityTypeConfiguration<TransactionV2>
{
    public override void ConfigureDotBoilEntity(EntityTypeBuilder<TransactionV2> builder)
    {
        builder
            .Property(p => p.NotebookV2Id)
            .IsRequired();

        builder
            .Property(p => p.ExternalId)
            .IsRequired()
            .HasMaxLength(128);

        builder
            .Property(p => p.Name)
            .IsRequired()
            .HasMaxLength(128);

        builder
            .Property(p => p.Description)
            .IsRequired()
            .HasMaxLength(256);

        builder
            .Property(p => p.Amount)
            .IsRequired();

        builder
            .Property(p => p.CurrencyId)
            .IsRequired();

        builder
            .Property(p => p.TransactionType)
            .IsRequired();

        builder
            .Property(p => p.IsMatched)
            .IsRequired();

        builder
            .Property(p => p.IsProceed)
            .IsRequired();

        builder
            .Property(p => p.TransactionDate)
            .IsRequired();

        builder
            .HasOne<Currency>(p => p.Currency)
            .WithMany(p => p.Transactions)
            .HasForeignKey(p => p.CurrencyId)
            .OnDelete(DeleteBehavior.NoAction);

        builder
            .ToTable("TransactionsV2");
    }
}