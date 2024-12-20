using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SachkovTech.Issues.Application.DataModels;

namespace SachkovTech.Issues.Infrastructure.Configurations.Read;

public class UserIssueDataModelConfiguration : IEntityTypeConfiguration<UserIssueDataModel>
{
    public void Configure(EntityTypeBuilder<UserIssueDataModel> builder)
    {
        builder.ToTable("user_issues");

        builder.HasKey(u => u.Id);
    }
}