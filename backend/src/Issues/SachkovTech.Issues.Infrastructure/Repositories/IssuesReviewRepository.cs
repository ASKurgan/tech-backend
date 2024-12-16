using CSharpFunctionalExtensions;
using Microsoft.EntityFrameworkCore;
using SachkovTech.Issues.Application.Interfaces;
using SachkovTech.Issues.Domain.IssuesReviews;
using SachkovTech.Issues.Infrastructure.DbContexts;
using SachkovTech.SharedKernel;
using SachkovTech.SharedKernel.ValueObjects.Ids;
using SharedKernel;

namespace SachkovTech.Issues.Infrastructure.Repositories;

public class IssuesReviewRepository : IIssuesReviewRepository
{
    private readonly IssuesWriteDbContext _dbContext;

    public IssuesReviewRepository(IssuesWriteDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<Result<IssueReview, Error>> GetById(IssueReviewId id,
        CancellationToken cancellationToken = default)
    {
        var issueReview = await _dbContext.IssueReviews
            .Include(ir => ir.Comments)
            .FirstOrDefaultAsync(i => i.Id == id, cancellationToken);

        if (issueReview == null)
            return Errors.General.NotFound(id);

        return issueReview;
    }

    public async Task<Result<IssueReview, Error>> GetByUserIssueId(UserIssueId id,
        CancellationToken cancellationToken = default)
    {
        var issueReview = await _dbContext.IssueReviews
            .FirstOrDefaultAsync(i => i.UserIssueId == id, cancellationToken);

        if (issueReview == null)
            return Errors.General.NotFound(id);

        return issueReview;
    }

    public async Task<UnitResult<Error>> Add(IssueReview issueReview, CancellationToken cancellationToken = default)
    {
        await _dbContext.AddAsync(issueReview, cancellationToken);

        return UnitResult.Success<Error>();
    }
}