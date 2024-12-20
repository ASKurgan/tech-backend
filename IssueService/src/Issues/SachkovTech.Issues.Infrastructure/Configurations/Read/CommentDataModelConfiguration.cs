using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SachkovTech.Issues.Application.DataModels;

namespace SachkovTech.Issues.Infrastructure.Configurations.Read;

public class CommentDataModelConfiguration : IEntityTypeConfiguration<CommentDataModel>
{
    public void Configure(EntityTypeBuilder<CommentDataModel> builder)
    {
        builder.ToTable("comments");

        builder.HasKey(c => c.Id);
    }
}