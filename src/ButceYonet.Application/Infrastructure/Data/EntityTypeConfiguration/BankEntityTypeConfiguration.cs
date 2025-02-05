using ButceYonet.Application.Domain.Entities;
using DotBoil.EFCore;
using DotBoil.EFCore.Attributes;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ButceYonet.Application.Infrastructure.Data.EntityTypeConfiguration;

[DotBoilEntityTypeConfiguration(typeof(ButceYonetDbContext))]
public class BankEntityTypeConfiguration : EFCoreEntityTypeConfiguration<Bank>
{
    public override void ConfigureDotBoilEntity(EntityTypeBuilder<Bank> builder)
    {
        builder
            .Property(p => p.Bid)
            .HasMaxLength(10)
            .IsRequired();

        builder
            .Property(p => p.Name)
            .HasMaxLength(100)
            .IsRequired();

        builder
            .Property(p => p.Description)
            .HasMaxLength(256)
            .IsRequired();

        builder
            .Property(p => p.TypeName)
            .HasMaxLength(256)
            .IsRequired();
        
        builder
            .HasMany<BankAccount>(p => p.BankAccounts)
            .WithOne(p => p.Bank)
            .HasForeignKey(p => p.BankId)
            .OnDelete(DeleteBehavior.NoAction);
        
        builder
            .ToTable("Banks");
    }
}