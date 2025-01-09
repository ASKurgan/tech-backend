using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ScheduleService.Domain.Entities;
using ScheduleService.Domain.Ids;

namespace ScheduleService.Infrastructure.Configurations.Write;
public class EventInstanceWriteConfiguration : IEntityTypeConfiguration<EventInstance>
{
    public void Configure(EntityTypeBuilder<EventInstance> builder)
    { 
        builder.ToTable("events");

        builder.HasKey(e => e.Id);

        builder.Property(e => e.Id)
            .HasConversion(
            id => id.Value,
            value => EventId.Create(value))
            .HasColumnName("id");       

        builder.Property(e => e.Start)
            .IsRequired()
            .HasColumnName("start");

        builder.Property(e => e.Duration)
            .IsRequired()
            .HasColumnName("duration");

        builder.Property(e=>e.Status)
            .IsRequired()
            .HasColumnName("status");

        builder.Property(e => e.ScheduleId)
            .HasConversion(
             id => id.Value,
             value => ScheduleId.Create(value))
            .HasColumnName("schedule_id");        
    }
}
