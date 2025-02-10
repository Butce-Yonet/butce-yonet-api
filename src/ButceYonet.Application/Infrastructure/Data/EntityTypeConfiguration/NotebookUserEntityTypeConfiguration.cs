using ButceYonet.Application.Domain.Entities;
using DotBoil.EFCore;
using DotBoil.EFCore.Attributes;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ButceYonet.Application.Infrastructure.Data.EntityTypeConfiguration;

[DotBoilEntityTypeConfiguration(typeof(NotebookUser))]
public class NotebookUserEntityTypeConfiguration : EFCoreEntityTypeConfiguration<NotebookUser>
{
    public override void ConfigureDotBoilEntity(EntityTypeBuilder<NotebookUser> builder)
    {
        builder
            .Property(p => p.UserId)
            .IsRequired();

        builder
            .Property(p => p.NotebookId)
            .IsRequired();

        builder
            .Property(p => p.IsDefault)
            .IsRequired();

        builder
            .HasOne<Notebook>(p => p.Notebook)
            .WithMany(p => p.NotebookUsers)
            .HasForeignKey(p => p.NotebookId)
            .OnDelete(DeleteBehavior.NoAction);
        
        builder.ToTable("NotebookUsers");
    }
}