using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using UserProgressService.Domain.Progress;
using UserProgressService.Domain.ValueObjects.Ids;

namespace UserProgressService.Infrastructure.Configuration;

public class IssueProgressConfiguration : IEntityTypeConfiguration<IssueProgress>
{
    public void Configure(EntityTypeBuilder<IssueProgress> builder)
    {
        builder.ToTable("issue_progresses");

        builder.HasKey(i => i.Id);
        builder.Property(i => i.Id)
            .HasConversion(
                id => id.Value,
                value => IssueProgressId.Create(value));

        builder.Property(i => i.ModuleId)
            .IsRequired(false);

        builder.Property(i => i.IssueId)
            .IsRequired();

        builder.Property(i => i.TryCount)
            .IsRequired();

        builder.Property(i => i.StartedDate)
            .IsRequired();

        builder.Property(i => i.ExecutionTime)
            .IsRequired();

        builder.ComplexProperty(i => i.Difficulty, b =>
        {
            b.Property(d => d.Value).IsRequired();
        });
    }
}