using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ScheduleService.Domain.Ids;
using ScheduleService.Domain.TypeSchedules;
using System.Text.Json;

namespace ScheduleService.Infrastructure.Configurations.Write;
public class DailyScheduleConfiguration : IEntityTypeConfiguration<DailySchedule>
{
    public void Configure(EntityTypeBuilder<DailySchedule> builder)
    {
        builder.ToTable("daily_schedules");

        builder.HasKey(d => d.Id);

        builder.Property(d => d.Id)
            .HasConversion(
            id => id.Value,
            value => ScheduleId.Create(value))
            .HasColumnName("id");

        builder.Property(d => d.ExecutionTimes)
            .HasConversion(
                t => JsonSerializer.Serialize(t, JsonSerializerOptions.Default),
                json => JsonSerializer
                    .Deserialize<List<TimeSpan>>(json, JsonSerializerOptions.Default)!)
            .HasColumnName("execution_times");

        builder.Property(d => d.StartDate)
            .IsRequired()
            .HasColumnName("start_date");

        builder.Property(d => d.EndDate)
            .IsRequired()
            .HasColumnName("end_date");
        
        builder.ComplexProperty(d => d.Title,
               db =>
               {
                   db.Property(p => p.Value)
                   .IsRequired()
                   .HasColumnName("title");
               });

        builder.ComplexProperty(d => d.Description,
               db =>
               {
                   db.Property(p => p.Value)
                   .IsRequired()
                   .HasColumnName("description");
               });

        builder.Property(d => d.IsAutomaticRenewal)
            .IsRequired()
            .HasColumnName("is_automatic_renewal");

        builder.HasMany(d => d.PlannedEvents)
            .WithOne()
            .HasForeignKey("daily_scheduler_id");

        builder.Navigation(d => d.PlannedEvents)
            .AutoInclude();
    }
}
