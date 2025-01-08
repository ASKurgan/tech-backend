using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using UserProgressService.Domain.Progress;
using UserProgressService.Domain.ValueObjects.Ids;

namespace UserProgressService.Infrastructure.Configuration;

public class LessonProgressConfiguration : IEntityTypeConfiguration<LessonProgress>
{
    public void Configure(EntityTypeBuilder<LessonProgress> builder)
    {
        builder.ToTable("lesson_progresses");
        
        builder.HasKey(l => l.Id);
        builder.Property(l => l.Id)
            .HasConversion(
                id => id.Value,
                value => LessonProgressId.Create(value));
        
        builder.Property(l => l.ModuleId)
            .IsRequired();
        
        builder.Property(l => l.LessonId)
            .IsRequired(false);

        builder.Property(l => l.IsCompleted)
            .IsRequired();
    }
}