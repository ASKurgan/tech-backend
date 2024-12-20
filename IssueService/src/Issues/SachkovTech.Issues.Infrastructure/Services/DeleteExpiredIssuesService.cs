using Microsoft.EntityFrameworkCore;
using SachkovTech.Issues.Domain.Issue;
using SachkovTech.Issues.Infrastructure.DbContexts;

namespace SachkovTech.Issues.Infrastructure.Services;

public class DeleteExpiredIssuesService
{
    private readonly IssuesWriteDbContext _issuesWriteDbContext;

    public DeleteExpiredIssuesService(
        IssuesWriteDbContext issuesWriteDbContext)
    {
        _issuesWriteDbContext = issuesWriteDbContext;
    }

    public async Task Process(CancellationToken cancellationToken)
    {
        var issues = await GetModulesWithIssuesAsync(cancellationToken);

        issues.RemoveAll(i => i.DeletionDate != null
                              && DateTime.UtcNow >= i.DeletionDate.Value
                                  .AddDays(Constants.Issues.LIFETIME_AFTER_DELETION));

        await _issuesWriteDbContext.SaveChangesAsync(cancellationToken);
    }

    private async Task<List<Issue>> GetModulesWithIssuesAsync(CancellationToken cancellationToken)
    {
        return await _issuesWriteDbContext.Issues.ToListAsync(cancellationToken);
    }
}