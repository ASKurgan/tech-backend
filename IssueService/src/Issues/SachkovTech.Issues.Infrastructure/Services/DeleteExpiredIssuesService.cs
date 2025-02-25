using Microsoft.EntityFrameworkCore;
using SachkovTech.Issues.Domain.Issue;
using SachkovTech.Issues.Infrastructure.DbContexts;

namespace SachkovTech.Issues.Infrastructure.Services;

public class DeleteExpiredIssuesService
{
    private readonly IssuesDbContext _issuesDbContext;

    public DeleteExpiredIssuesService(
        IssuesDbContext issuesDbContext)
    {
        _issuesDbContext = issuesDbContext;
    }

    public async Task Process(CancellationToken cancellationToken)
    {
        var issues = await GetModulesWithIssuesAsync(cancellationToken);

        issues.RemoveAll(i => i.DeletionDate != null
                              && DateTime.UtcNow >= i.DeletionDate.Value
                                  .AddDays(Constants.Issues.LIFETIME_AFTER_DELETION));

        await _issuesDbContext.SaveChangesAsync(cancellationToken);
    }

    private async Task<List<Issue>> GetModulesWithIssuesAsync(CancellationToken cancellationToken)
    {
        return await _issuesDbContext.Issues.ToListAsync(cancellationToken);
    }
}