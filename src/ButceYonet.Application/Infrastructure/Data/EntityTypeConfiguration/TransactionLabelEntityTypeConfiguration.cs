using ButceYonet.Application.Domain.Entities;
using DotBoil.EFCore;
using DotBoil.EFCore.Attributes;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ButceYonet.Application.Infrastructure.Data.EntityTypeConfiguration;

[DotBoilEntityTypeConfiguration(typeof(ButceYonetDbContext))]
public class TransactionLabelEntityTypeConfiguration : EFCoreEntityTypeConfiguration<TransactionLabel>
{
    public override void ConfigureDotBoilEntity(EntityTypeBuilder<TransactionLabel> builder)
    {
        builder
            .Property(p => p.NotebookLabelId)
            .IsRequired();

        builder
            .Property(p => p.TransactionId)
            .IsRequired();

        builder
            .HasOne<NotebookLabel>(p => p.NotebookLabel)
            .WithMany(p => p.TransactionLabels)
            .HasForeignKey(p => p.NotebookLabelId)
            .OnDelete(DeleteBehavior.NoAction);
        
        builder
            .HasOne<Transaction>(p => p.Transaction)
            .WithMany(p => p.TransactionLabels)
            .HasForeignKey(p => p.TransactionId)
            .OnDelete(DeleteBehavior.NoAction);
        
        builder
            .ToTable("TransactionLabels");
    }
}