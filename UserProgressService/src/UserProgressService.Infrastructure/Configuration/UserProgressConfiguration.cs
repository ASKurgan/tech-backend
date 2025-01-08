using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SachkovTech.Core.Database;
using UserProgressService.Domain.Progress;
using UserProgressService.Domain.ValueObjects.Ids;

namespace UserProgressService.Infrastructure.Configuration;

public class UserProgressConfiguration : IEntityTypeConfiguration<UserProgress>
{
    public void Configure(EntityTypeBuilder<UserProgress> builder)
    {
        builder.ToTable("user_progresses");

        builder.HasKey(u => u.Id);
        builder.Property(u => u.Id)
            .HasConversion(
                id => id.Value,
                value => UserProgressId.Create(value));

        builder.Property(u => u.UserActivityDate)
            .IsRequired();

        builder.ComplexProperty(u => u.Level, b =>
        {
            b.Property(l => l.CurrentExperience)
                .IsRequired();

            b.Property(l => l.CurrentLevel)
                .IsRequired();
        });

        builder.Property(u => u.Achievements)
            .ValueObjectsCollectionJsonConversion(
            achievements => achievements,
            value => value);

        builder.HasMany(u => u.IssueProgresses)
            .WithOne()
            .HasForeignKey("issue_progress_id");

        builder.HasMany(u => u.LessonProgresses)
            .WithOne()
            .HasForeignKey("lesson_progress_id");
    }
}