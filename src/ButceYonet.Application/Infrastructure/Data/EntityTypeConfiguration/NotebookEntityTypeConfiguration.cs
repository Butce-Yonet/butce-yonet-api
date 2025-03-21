using ButceYonet.Application.Domain.Entities;
using DotBoil.EFCore;
using DotBoil.EFCore.Attributes;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ButceYonet.Application.Infrastructure.Data.EntityTypeConfiguration;

[DotBoilEntityTypeConfiguration(typeof(ButceYonetDbContext))]
public class NotebookEntityTypeConfiguration : EFCoreEntityTypeConfiguration<Notebook>
{
    public override void ConfigureDotBoilEntity(EntityTypeBuilder<Notebook> builder)
    {
        builder
            .Property(p => p.Name)
            .HasMaxLength(50)
            .IsRequired();

        builder
            .Property(p => p.IsDefault)
            .IsRequired();

        builder
            .HasMany<NotebookUser>(p => p.NotebookUsers)
            .WithOne(p => p.Notebook)
            .HasForeignKey(p => p.NotebookId)
            .OnDelete(DeleteBehavior.NoAction);

        builder
            .HasMany<NotebookLabel>(p => p.NotebookLabels)
            .WithOne(p => p.Notebook)
            .HasForeignKey(p => p.NotebookId)
            .OnDelete(DeleteBehavior.NoAction);

        builder
            .HasMany<Transaction>(p => p.Transactions)
            .WithOne(p => p.Notebook)
            .HasForeignKey(p => p.NotebookId)
            .OnDelete(DeleteBehavior.NoAction);

        builder
            .HasMany<BankAccount>(p => p.BankAccounts)
            .WithOne(p => p.Notebook)
            .HasForeignKey(p => p.NotebookId)
            .OnDelete(DeleteBehavior.NoAction);
        
        builder
            .HasMany<CategorizedTransactionReport>(p => p.CategorizedTransactionReports)
            .WithOne(p => p.Notebook)
            .HasForeignKey(p => p.NotebookId)
            .OnDelete(DeleteBehavior.NoAction);
        
        builder
            .HasMany<NonCategorizedTransactionReport>(p => p.NonCategorizedTransactionReports)
            .WithOne(p => p.Notebook)
            .HasForeignKey(p => p.NotebookId)
            .OnDelete(DeleteBehavior.NoAction);
            
        builder.ToTable("Notebooks");
    }
}