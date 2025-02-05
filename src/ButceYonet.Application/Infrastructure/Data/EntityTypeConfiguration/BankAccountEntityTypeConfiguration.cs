using ButceYonet.Application.Domain.Entities;
using DotBoil.EFCore;
using DotBoil.EFCore.Attributes;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ButceYonet.Application.Infrastructure.Data.EntityTypeConfiguration;

[DotBoilEntityTypeConfiguration(typeof(ButceYonetDbContext))]
public class BankAccountEntityTypeConfiguration : EFCoreEntityTypeConfiguration<BankAccount>
{
    public override void ConfigureDotBoilEntity(EntityTypeBuilder<BankAccount> builder)
    {
        builder
            .Property(p => p.BankId)
            .IsRequired();

        builder
            .Property(p => p.Description)
            .HasMaxLength(128)
            .IsRequired();

        builder
            .HasOne<Bank>(p => p.Bank)
            .WithMany(p => p.BankAccounts)
            .HasForeignKey(p => p.BankId)
            .OnDelete(DeleteBehavior.NoAction);
        
        builder
            .ToTable("BankAccounts");
    }
}