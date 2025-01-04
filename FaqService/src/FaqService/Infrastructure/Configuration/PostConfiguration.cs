using FaqService.Entities;
using FaqService.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using static FaqService.Constants.Constants;

namespace FaqService.Infrastructure.Configuration;

public class PostConfiguration : IEntityTypeConfiguration<Post>
{
    public void Configure(EntityTypeBuilder<Post> builder)
    {
        builder.ToTable("posts");

        builder.HasKey(x => x.Id);

        builder.Property(p => p.Title)
            .HasMaxLength(LOW_TEXT_LENGTH)
            .HasColumnName("title")
            .IsRequired();

        builder.Property(p => p.Description)
            .HasMaxLength(MAX_TEXT_LENGTH)
            .HasColumnName("description")
            .IsRequired();

        builder.Property(p => p.ReplLink)
            .HasMaxLength(LOW_TEXT_LENGTH)
            .HasColumnName("rep_link")
            .IsRequired();

        builder.Property(p => p.UserId)
            .HasColumnName("user_id")
            .IsRequired();

        builder.Property(p => p.IssueId)
            .HasColumnName("issue_id")
            .IsRequired(false);

        builder.Property(p => p.LessonId)
            .HasColumnName("lesson_id")
            .IsRequired(false);

        builder.Property(p => p.Tags)
            .HasColumnName("tags")
            .IsRequired(false);

        builder.Property(p => p.Status)
            .HasConversion(
                status => status.ToString(),
                value =>(Status)Enum.Parse(typeof(Status), value))
            .HasColumnType("text")
            .HasColumnName("status")
            .IsRequired();

        builder.Property(p => p.AnswerId)
            .HasColumnName("answer_id")
            .IsRequired(false);

        builder.Property(p => p.CreatedAt)
            .HasColumnName("created_at")
            .IsRequired();
        
        builder.HasMany(b => b.Answers)
            .WithOne()
            .HasForeignKey("post_id")
            .IsRequired()
            .OnDelete(DeleteBehavior.Cascade);

    }
}