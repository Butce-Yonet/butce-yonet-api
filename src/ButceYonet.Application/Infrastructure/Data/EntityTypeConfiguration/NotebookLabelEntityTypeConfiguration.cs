using ButceYonet.Application.Domain.Entities;
using DotBoil.EFCore;
using DotBoil.EFCore.Attributes;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ButceYonet.Application.Infrastructure.Data.EntityTypeConfiguration;

[DotBoilEntityTypeConfiguration(typeof(ButceYonetDbContext))]
public class NotebookLabelEntityTypeConfiguration : EFCoreEntityTypeConfiguration<NotebookLabel>
{
    public override void ConfigureDotBoilEntity(EntityTypeBuilder<NotebookLabel> builder)
    {
        builder
            .Property(p => p.Name)
            .HasMaxLength(128)
            .IsRequired();

        builder
            .Property(p => p.ColorCode)
            .HasMaxLength(16)
            .IsRequired();

        builder
            .HasOne<Notebook>(p => p.Notebook)
            .WithMany(p => p.NotebookLabels)
            .HasForeignKey(p => p.NotebookId)
            .OnDelete(DeleteBehavior.NoAction);

        builder
            .HasMany<TransactionLabel>(p => p.TransactionLabels)
            .WithOne(p => p.NotebookLabel)
            .HasForeignKey(p => p.NotebookLabelId)
            .OnDelete(DeleteBehavior.NoAction);
        
        builder
            .HasMany<CategorizedTransactionReport>(p => p.CategorizedTransactionReports)
            .WithOne(p => p.NotebookLabel)
            .HasForeignKey(p => p.NotebookLabelId)
            .OnDelete(DeleteBehavior.NoAction);
        
        builder
            .ToTable("NotebookLabels");
    }
}