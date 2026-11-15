using ButceYonet.Application.Domain.Entities;
using DotBoil.EFCore;
using DotBoil.EFCore.Attributes;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ButceYonet.Application.Infrastructure.Data.EntityTypeConfiguration;

[DotBoilEntityTypeConfiguration(typeof(ButceYonetAuthorizationDbContext))]
public class UserNotificationPreferenceEntityTypeConfiguration : EFCoreEntityTypeConfiguration<UserNotificationPreference>
{
    public override void ConfigureDotBoilEntity(EntityTypeBuilder<UserNotificationPreference> builder)
    {
        builder
            .Property(p => p.UserId)
            .IsRequired();

        builder
            .Property(p => p.Channel)
            .HasMaxLength(50)
            .IsRequired();

        builder
            .Property(p => p.IsEnabled)
            .IsRequired();

        builder.ToTable("UserNotificationPreferences");
    }
}
