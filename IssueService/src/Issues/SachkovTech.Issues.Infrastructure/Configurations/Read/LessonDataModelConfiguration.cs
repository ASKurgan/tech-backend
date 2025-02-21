using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SachkovTech.Issues.Application.DataModels;

namespace SachkovTech.Issues.Infrastructure.Configurations.Read;

public class LessonDataModelConfiguration : IEntityTypeConfiguration<LessonDataModel>
{
    public void Configure(EntityTypeBuilder<LessonDataModel> builder)
    {
        builder.ToTable("lessons");

        builder.HasKey(l => l.Id)
            .HasName("id");

        builder.Property(l => l.ModuleId)
            .IsRequired()
            .HasColumnName("module_id");

        builder.Property(t => t.Title)
            .IsRequired()
            .HasColumnName("title");

        builder.Property(d => d.Description)
            .IsRequired()
            .HasColumnName("description");

        builder.Property(b => b.Experience)
            .HasColumnName("experience");

        builder.Property(l => l.FileId)
            .IsRequired()
            .HasColumnName("file_id");

        builder.Property(l => l.FileLocation)
            .IsRequired()
            .HasColumnName("file_location");

        builder.Property(l => l.PreviewId)
            .IsRequired(false)
            .HasColumnName("preview_id");

        builder.Property(l => l.Tags)
            .HasColumnName("tags");

        builder.Property(l => l.Issues)
            .HasColumnName("issues");
    }
}