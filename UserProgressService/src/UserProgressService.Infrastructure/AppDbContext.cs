using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace UserProgressService.Infrastructure;

public class AppDbContext : DbContext
{
    private readonly IConfiguration _configuration;
    
    private const string DATABASE = "Database";

    public AppDbContext(IConfiguration configuration)
    {
        _configuration = configuration;
    }
    
    // DbSet<IssueAchievement> IssueAchievement => Set<IssueAchievement>();
    // DbSet<UserIssuesProgress> UserIssuesProgress => Set<UserIssuesProgress>();
    // DbSet<UserProgress> UserProgress => Set<UserProgress>();
    
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseNpgsql(_configuration.GetConnectionString(DATABASE));
        optionsBuilder.UseSnakeCaseNamingConvention();
        optionsBuilder.EnableSensitiveDataLogging();
        optionsBuilder.UseLoggerFactory(CreateLoggerFactory());
        optionsBuilder.ConfigureWarnings(warnings =>
            warnings.Ignore(RelationalEventId.PendingModelChangesWarning));
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);
    }

    private ILoggerFactory CreateLoggerFactory() =>
        LoggerFactory.Create(builder => { builder.AddConsole(); });
}