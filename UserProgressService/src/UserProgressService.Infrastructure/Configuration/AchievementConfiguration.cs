using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Newtonsoft.Json;
using UserProgressService.Domain.Achievements;
using UserProgressService.Domain.ValueObjects.Conditions;
using UserProgressService.Domain.ValueObjects.Ids;

namespace UserProgressService.Infrastructure.Configuration;

public class AchievementConfiguration : IEntityTypeConfiguration<Achievement>
{
    public void Configure(EntityTypeBuilder<Achievement> builder)
    {
        builder.ToTable("achievements");

        builder.HasKey(a => a.Id);
        builder.Property(a => a.Id)
            .HasConversion(
                id => id.Value,
                value => AchievementId.Create(value));

        builder.Property(a => a.IconId)
            .IsRequired();

        builder.Property(a => a.Name)
            .IsRequired();

        builder.Property(a => a.Description)
            .IsRequired();

        builder.Property(a => a.CreatedDate)
            .IsRequired();

        builder.Property(a => a.Experience)
            .IsRequired();

        var settings = new JsonSerializerSettings { TypeNameHandling = TypeNameHandling.All };

        builder.Property(a => a.Condition)
            .HasColumnType("jsonb")
            .HasConversion(
                condition => JsonConvert.SerializeObject(condition, settings),
                json => JsonConvert.DeserializeObject<Condition>(json, settings)!)
            .IsRequired();
    }
}