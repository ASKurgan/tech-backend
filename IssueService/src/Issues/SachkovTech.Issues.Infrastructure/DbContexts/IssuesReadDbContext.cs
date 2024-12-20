using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SachkovTech.Issues.Application.DataModels;
using SachkovTech.Issues.Application.Interfaces;

namespace SachkovTech.Issues.Infrastructure.DbContexts;

public class IssuesReadDbContext : DbContext, IReadDbContext
{
    private readonly string _connectionString;

    public IssuesReadDbContext(string connectionString)
    {
        _connectionString = connectionString;
    }

    public IQueryable<IssueDataModel> Issues => Set<IssueDataModel>();

    public IQueryable<ModuleDataModel> Modules => Set<ModuleDataModel>();

    public IQueryable<IssueReviewDataModel> IssueReviewDtos => Set<IssueReviewDataModel>();

    public IQueryable<CommentDataModel> Comments => Set<CommentDataModel>();

    public IQueryable<UserIssueDataModel> UserIssues => Set<UserIssueDataModel>();
    public IQueryable<LessonDataModel> Lessons => Set<LessonDataModel>();

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseNpgsql(_connectionString);
        optionsBuilder.UseSnakeCaseNamingConvention();
        optionsBuilder.EnableSensitiveDataLogging();
        optionsBuilder.UseLoggerFactory(CreateLoggerFactory());

        optionsBuilder.UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking);
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema("issues");

        modelBuilder.ApplyConfigurationsFromAssembly(
            typeof(IssuesWriteDbContext).Assembly,
            type => type.FullName?.Contains("Configurations.Read") ?? false);
    }

    private ILoggerFactory CreateLoggerFactory() =>
        LoggerFactory.Create(builder => { builder.AddConsole(); });
}