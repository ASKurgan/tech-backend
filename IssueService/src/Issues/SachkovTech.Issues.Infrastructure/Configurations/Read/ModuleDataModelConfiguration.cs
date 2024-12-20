using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SachkovTech.Issues.Application.DataModels;
using SachkovTech.Issues.Contracts.Dtos;

namespace SachkovTech.Issues.Infrastructure.Configurations.Read;

public class ModuleDataModelConfiguration : IEntityTypeConfiguration<ModuleDataModel>
{
    public void Configure(EntityTypeBuilder<ModuleDataModel> builder)
    {
        builder.ToTable("modules");

        builder.HasKey(m => m.Id);

        builder.HasQueryFilter(m => m.IsDeleted == false);

        builder.Property(m => m.IssuesPosition)
            .HasConversion(
                issues => string.Empty,
                json => JsonSerializer.Deserialize<IssuePositionDto[]>(json, JsonSerializerOptions.Default) ??
                        Array.Empty<IssuePositionDto>())
            .HasColumnType("jsonb");

        builder.Property(m => m.LessonsPosition)
            .HasConversion(
                lessons => string.Empty,
                json => JsonSerializer.Deserialize<LessonPositionDto[]>(json, JsonSerializerOptions.Default) ??
                        Array.Empty<LessonPositionDto>())
            .HasColumnType("jsonb");
    }
}