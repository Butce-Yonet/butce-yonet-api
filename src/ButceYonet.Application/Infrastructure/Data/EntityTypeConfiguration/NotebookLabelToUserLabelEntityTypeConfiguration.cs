using ButceYonet.Application.Domain.Entities;
using DotBoil.EFCore;
using DotBoil.EFCore.Attributes;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ButceYonet.Application.Infrastructure.Data.EntityTypeConfiguration;

[DotBoilEntityTypeConfiguration(typeof(ButceYonetDbContext))]
public class NotebookLabelToUserLabelEntityTypeConfiguration : EFCoreEntityTypeConfiguration<NotebookLabelToUserLabel>
{
    public override void ConfigureDotBoilEntity(EntityTypeBuilder<NotebookLabelToUserLabel> builder)
    {
        builder
            .Property(p => p.NotebookLabelId)
            .IsRequired();

        builder
            .Property(p => p.UserLabelId)
            .IsRequired();

        builder
            .HasOne<NotebookLabel>(p => p.NotebookLabel)
            .WithMany(p => p.NotebookLabelToUserLabels)
            .HasForeignKey(p => p.NotebookLabelId)
            .OnDelete(DeleteBehavior.NoAction);

        builder
            .HasOne<UserLabel>(p => p.UserLabel)
            .WithMany(p => p.NotebookLabelToUserLabels)
            .HasForeignKey(p => p.UserLabelId)
            .OnDelete(DeleteBehavior.NoAction);

        builder
            .ToTable("NotebookLabelToUserLabels");
    }
}