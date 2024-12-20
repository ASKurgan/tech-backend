using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SachkovTech.Issues.Application.DataModels;

namespace SachkovTech.Issues.Infrastructure.Configurations.Read;

public class IssueReviewDataModelConfiguration : IEntityTypeConfiguration<IssueReviewDataModel>
{
    public void Configure(EntityTypeBuilder<IssueReviewDataModel> builder)
    {
        builder.ToTable("issue_reviews");

        builder.HasKey(i => i.Id);

        builder.HasMany(i => i.Comments)
            .WithOne()
            .HasForeignKey(i => i.IssueReviewId);
    }
}