using ButceYonet.Application.Domain.Entities;
using DotBoil.EFCore;
using DotBoil.EFCore.Attributes;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ButceYonet.Application.Infrastructure.Data.EntityTypeConfiguration;

[DotBoilEntityTypeConfiguration(typeof(ButceYonetAuthorizationDbContext))]
public class UserEntityTypeConfiguration : EFCoreEntityTypeConfiguration<User>
{
    public override void ConfigureDotBoilEntity(EntityTypeBuilder<User> builder)
    {
        builder
            .Property(p => p.Email)
            .HasMaxLength(100)
            .IsRequired(false);
        
        builder.ToTable("Users");
    }
}