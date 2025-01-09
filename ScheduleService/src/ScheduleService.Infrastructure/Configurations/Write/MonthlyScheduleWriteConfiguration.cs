using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ScheduleService.Domain.Ids;
using ScheduleService.Domain.TypeSchedules;
using System.Text.Json;

namespace ScheduleService.Infrastructure.Configurations.Write;
public class MonthlyScheduleWriteConfiguration : IEntityTypeConfiguration<MonthlySchedule>
{
    public void Configure(EntityTypeBuilder<MonthlySchedule> builder)
    {
        builder.ToTable("monthly_schedules");

        builder.HasKey(w => w.Id);

        builder.Property(w => w.Id)
            .HasConversion(
            id => id.Value,
            value => ScheduleId.Create(value))
            .HasColumnName("id");

        builder.Property(w => w.ExecutionDays)
            .HasConversion(
                t => JsonSerializer.Serialize(t, JsonSerializerOptions.Default),
                json => JsonSerializer
                    .Deserialize<List<DateTime>>(json, JsonSerializerOptions.Default)!)
            .HasColumnName("execution_days");

        builder.Property(w => w.RepeatInterval)
            .IsRequired()
            .HasColumnName("repeated_interval");

        builder.Property(w => w.StartDate)
            .IsRequired()
            .HasColumnName("start_date");

        builder.Property(w => w.EndDate)
            .IsRequired()
            .HasColumnName("end_date");

        builder.ComplexProperty(w => w.Title,
               db =>
               {
                   db.Property(p => p.Value)
                   .IsRequired()
                   .HasColumnName("title");
               });

        builder.ComplexProperty(w => w.Description,
               db =>
               {
                   db.Property(p => p.Value)
                   .IsRequired()
                   .HasColumnName("description");
               });

        builder.Property(w => w.IsAutomaticRenewal)
            .IsRequired()
            .HasColumnName("is_automatic_renewal");

        builder.HasMany(w => w.PlannedEvents)
            .WithOne()
            .HasForeignKey("monthly_scheduler_id");

        builder.Navigation(w => w.PlannedEvents)
            .AutoInclude();
    }
}
