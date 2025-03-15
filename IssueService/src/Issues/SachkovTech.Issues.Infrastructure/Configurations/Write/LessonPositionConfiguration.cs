using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SachkovTech.Issues.Domain.Module.Entities;
using SachkovTech.Issues.Domain.ValueObjects.Ids;

namespace SachkovTech.Issues.Infrastructure.Configurations.Write;

public class LessonPositionConfiguration : IEntityTypeConfiguration<LessonPosition>
{
    public void Configure(EntityTypeBuilder<LessonPosition> builder)
    {
        builder.ToTable("lesson_position");

        builder.HasKey(l => l.Id);

        builder.Property(l => l.Id)
            .HasConversion(
                id => id.Value,
                value => LessonPositionId.Create(value));

        builder.ComplexProperty(l => l.LessonId, b =>
        {
            b.Property(id => id.Value)
                .HasColumnName("lesson_id")
                .IsRequired();
        });

        builder.ComplexProperty(l => l.Position, b =>
        {
            b.Property(p => p.Value)
                .HasColumnName("position")
                .IsRequired();
        });
    }
}