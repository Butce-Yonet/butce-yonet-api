using ButceYonet.Application.Domain.Entities;
using DotBoil.EFCore;
using DotBoil.EFCore.Attributes;
using Microsoft.AspNetCore.Hosting.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ButceYonet.Application.Infrastructure.Data.EntityTypeConfiguration;

[DotBoilEntityTypeConfiguration(typeof(ButceYonetDbContext))]
public class TransactionEntityTypeConfiguration : EFCoreEntityTypeConfiguration<Transaction>
{
    public override void ConfigureDotBoilEntity(EntityTypeBuilder<Transaction> builder)
    {
        builder
            .Property(p => p.NotebookId);

        builder
            .Property(p => p.BankAccountId);

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
            .HasOne<Notebook>(p => p.Notebook)
            .WithMany(p => p.Transactions)
            .HasForeignKey(p => p.NotebookId)
            .OnDelete(DeleteBehavior.NoAction);

        builder
            .HasOne<BankAccount>(p => p.BankAccount)
            .WithMany(p => p.Transactions)
            .HasForeignKey(p => p.BankAccountId)
            .OnDelete(DeleteBehavior.NoAction);

        builder
            .HasOne<Currency>(p => p.Currency)
            .WithMany(p => p.Transactions)
            .HasForeignKey(p => p.CurrencyId)
            .OnDelete(DeleteBehavior.NoAction);

        builder
            .HasMany<TransactionLabel>(p => p.TransactionLabels)
            .WithOne(p => p.Transaction)
            .HasForeignKey(p => p.TransactionId)
            .OnDelete(DeleteBehavior.NoAction);
        
        builder
            .ToTable("Transactions");
    }
}