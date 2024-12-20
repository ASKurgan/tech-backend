using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SachkovTech.Issues.Infrastructure.DbContexts;
using IMigrator = SachkovTech.Core.Database.IMigrator;

namespace SachkovTech.Issues.Infrastructure.Migrator;

public class IssuesMigrator(IssuesWriteDbContext context, ILogger<IssuesMigrator> logger) : IMigrator
{
    public async Task Migrate(CancellationToken cancellationToken = default)
    {
        logger.Log(LogLevel.Information, "Applying issues migrations...");
        
        if (await context.Database.CanConnectAsync(cancellationToken) == false)
        {
            await context.Database.EnsureCreatedAsync(cancellationToken);
        }

        await context.Database.MigrateAsync(cancellationToken);
        
        logger.Log(LogLevel.Information, "Migrations issues applied successfully.");
    }
}