using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SachkovTech.Issues.Domain.Module.Entities;
using SachkovTech.Issues.Domain.ValueObjects.Ids;

namespace SachkovTech.Issues.Infrastructure.Configurations.Write;

public class IssuePositionConfiguration : IEntityTypeConfiguration<IssuePosition>
{
    public void Configure(EntityTypeBuilder<IssuePosition> builder)
    {
        builder.ToTable("issue_positions");

        builder.HasKey(i => i.Id);

        builder.Property(i => i.Id)
            .HasConversion(
                id => id.Value,
                value => IssuePositionId.Create(value));

        builder.ComplexProperty(i => i.IssueId, b =>
        {
            b.Property(id => id.Value)
                .HasColumnName("issue_id")
                .IsRequired();
        });

        builder.ComplexProperty(i => i.Position, b =>
        {
            b.Property(p => p.Value)
                .HasColumnName("position")
                .IsRequired();
        });
    }
}