using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using UserProgressService.Application.Interfaces;
using UserProgressService.Domain.Achievements;
using UserProgressService.Domain.Progress;

namespace UserProgressService.Infrastructure;

public class AppDbContext : DbContext, IDbContext
{
    private readonly IConfiguration _configuration;

    private const string DATABASE = "Database";

    public AppDbContext(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public DbSet<UserProgress> UserProgresses => Set<UserProgress>();
    public DbSet<Achievement> Achievements => Set<Achievement>();

    public IQueryable<UserProgress> UserProgressesQuery => Set<UserProgress>();
    public IQueryable<Achievement> AchievementsQuery => Set<Achievement>();

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

        base.OnModelCreating(modelBuilder);
    }

    private ILoggerFactory CreateLoggerFactory() =>
        LoggerFactory.Create(builder => { builder.AddConsole(); });
}