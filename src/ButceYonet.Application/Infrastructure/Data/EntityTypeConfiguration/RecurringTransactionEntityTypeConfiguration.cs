using ButceYonet.Application.Domain.Entities;
using DotBoil.EFCore;
using DotBoil.EFCore.Attributes;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ButceYonet.Application.Infrastructure.Data.EntityTypeConfiguration;

[DotBoilEntityTypeConfiguration((typeof(ButceYonetDbContext)))]
public class RecurringTransactionEntityTypeConfiguration : EFCoreEntityTypeConfiguration<RecurringTransaction>
{
    public override void ConfigureDotBoilEntity(EntityTypeBuilder<RecurringTransaction> builder)
    {
        builder
            .Property(p => p.NotebookId)
            .IsRequired();

        builder
            .Property(p => p.Name)
            .IsRequired()
            .HasMaxLength(128);

        builder
            .Property(p => p.Description)
            .IsRequired()
            .HasMaxLength(256);

        builder
            .Property(p => p.StartDate)
            .IsRequired();

        builder
            .Property(p => p.EndDate);

        builder
            .Property(p => p.Frequency)
            .IsRequired();

        builder
            .Property(p => p.Interval);

        builder
            .Property(p => p.NextOccurrence);

        builder
            .Property(p => p.StateData)
            .IsRequired();
        
        builder
            .ToTable("RecurringTransactions");
    }
}