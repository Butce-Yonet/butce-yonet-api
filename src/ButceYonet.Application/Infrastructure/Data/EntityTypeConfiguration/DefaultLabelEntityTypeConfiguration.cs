using ButceYonet.Application.Domain.Entities;
using DotBoil.EFCore;
using DotBoil.EFCore.Attributes;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ButceYonet.Application.Infrastructure.Data.EntityTypeConfiguration;

[DotBoilEntityTypeConfiguration(typeof(ButceYonetDbContext))]
public class DefaultLabelEntityTypeConfiguration : EFCoreEntityTypeConfiguration<DefaultLabel>
{
    public override void ConfigureDotBoilEntity(EntityTypeBuilder<DefaultLabel> builder)
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
            .ToTable("DefaultLabels");
    }
}