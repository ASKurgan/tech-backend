using CSharpFunctionalExtensions;
using Microsoft.EntityFrameworkCore;
using SachkovTech.Issues.Application.Interfaces;
using SachkovTech.Issues.Domain.IssueSolving.Entities;
using SachkovTech.Issues.Infrastructure.DbContexts;
using SachkovTech.SharedKernel.ValueObjects.Ids;
using SharedKernel;

namespace SachkovTech.Issues.Infrastructure.Repositories;

public class UserIssueRepository : IUserIssueRepository
{
    private readonly IssuesWriteDbContext _dbContext;

    public UserIssueRepository(IssuesWriteDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<Guid> Add(UserIssue userIssue, CancellationToken cancellationToken = default)
    {
        await _dbContext.UserIssues.AddAsync(userIssue, cancellationToken);
        await _dbContext.SaveChangesAsync(cancellationToken);

        return userIssue.Id;
    }

    public async Task<Result<UserIssue, Error>> GetUserIssueById(UserIssueId userIssueId,
        CancellationToken cancellationToken = default)
    {
        var userIssue =
            await _dbContext.UserIssues.SingleOrDefaultAsync(ui => ui.Id == userIssueId, cancellationToken);

        if (userIssue is null)
        {
            return Errors.General.NotFound();
        }

        return userIssue;
    }
}