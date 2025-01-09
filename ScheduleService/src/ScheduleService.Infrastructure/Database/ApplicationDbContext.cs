using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Configuration;
using ScheduleService.Domain.Entities;
using ScheduleService.Domain.TypeSchedules;

namespace ScheduleService.Infrastructure.Database;
public class ApplicationDbContext(IConfiguration configuration) : DbContext
{
    public const string DATABASE = "Database";

    public DbSet<DailySchedule> DailySchedules { get; set; }
    public DbSet<WeeklySchedule> WeeklySchedule { get; set; }
    public DbSet<MonthlySchedule> MonthlySchedule { get; set; }
    public DbSet<EventInstance> EventInstances { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseNpgsql(configuration.GetConnectionString(DATABASE));
        optionsBuilder.ConfigureWarnings(warnings =>
            warnings.Ignore(RelationalEventId.PendingModelChangesWarning));
        optionsBuilder.UseSnakeCaseNamingConvention();
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema("schedule");
        modelBuilder.ApplyConfigurationsFromAssembly(
            typeof(ApplicationDbContext).Assembly,
            type => type.FullName?.Contains("Configurations.Write") ?? false);
    }
}
