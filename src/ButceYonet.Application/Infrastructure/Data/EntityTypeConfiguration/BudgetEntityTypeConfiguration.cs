using ButceYonet.Application.Domain.Entities;
using DotBoil.EFCore;
using DotBoil.EFCore.Attributes;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ButceYonet.Application.Infrastructure.Data.EntityTypeConfiguration;

[DotBoilEntityTypeConfiguration(typeof(ButceYonetDbContext))]
public class BudgetEntityTypeConfiguration : EFCoreEntityTypeConfiguration<Budget>
{
    public override void ConfigureDotBoilEntity(EntityTypeBuilder<Budget> builder)
    {
        builder
            .Property(p => p.UserId)
            .IsRequired();

        builder
            .Property(p => p.Amount)
            .IsRequired();

        builder
            .Property(p => p.CurrencyId)
            .IsRequired();

        builder
            .Property(p => p.PeriodStartDay)
            .IsRequired()
            .HasDefaultValue(1);

        builder
            .HasOne(p => p.UserLabel)
            .WithMany()
            .HasForeignKey(p => p.UserLabelId)
            .OnDelete(DeleteBehavior.NoAction);

        builder
            .HasOne(p => p.Currency)
            .WithMany()
            .HasForeignKey(p => p.CurrencyId)
            .OnDelete(DeleteBehavior.NoAction);

        // MySQL filtrelenmiş (WHERE IsDeleted = 0) unique index desteklemediği ve NULL UserLabelId'ler
        // unique index'te birbirinden farklı sayıldığı için "aynı etiket + para birimine tek bütçe" kuralı
        // DB'de değil Create handler'ında uygulanıyor. Bu index sadece o kontrolü ve sinyal sorgusunu hızlandırır.
        builder
            .HasIndex(p => new { p.UserId, p.CurrencyId, p.UserLabelId });

        builder
            .ToTable("Budgets");
    }
}
