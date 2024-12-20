using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SachkovTech.Issues.Domain.Issue;
using SachkovTech.Issues.Domain.IssueSolving.Entities;
using SachkovTech.Issues.Domain.IssuesReviews;
using SachkovTech.Issues.Domain.Lesson;
using SachkovTech.Issues.Domain.Module;
using SachkovTech.Issues.Infrastructure.Outbox;

namespace SachkovTech.Issues.Infrastructure.DbContexts;

public class IssuesWriteDbContext : DbContext
{
    private readonly string _connectionString;

    public IssuesWriteDbContext(string connectionString)
    {
        _connectionString = connectionString;
    }

    public DbSet<Issue> Issues => Set<Issue>();

    public DbSet<Module> Modules => Set<Module>();

    public DbSet<UserIssue> UserIssues => Set<UserIssue>();

    public DbSet<IssueReview> IssueReviews => Set<IssueReview>();
    public DbSet<Lesson> Lessons => Set<Lesson>();

    public DbSet<OutboxMessage> OutboxMessages => Set<OutboxMessage>();

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseNpgsql(_connectionString);
        optionsBuilder.UseSnakeCaseNamingConvention();
        optionsBuilder.EnableSensitiveDataLogging();
        optionsBuilder.UseLoggerFactory(CreateLoggerFactory());
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema("issues");
        modelBuilder.ApplyConfigurationsFromAssembly(
            typeof(IssuesWriteDbContext).Assembly,
            type => type.FullName?.Contains("Configurations.Write") ?? false);
    }

    private ILoggerFactory CreateLoggerFactory() =>
        LoggerFactory.Create(builder => { builder.AddConsole(); });
}