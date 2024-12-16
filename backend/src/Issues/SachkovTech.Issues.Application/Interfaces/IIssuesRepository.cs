using CSharpFunctionalExtensions;
using SachkovTech.Issues.Domain.Issue;
using SachkovTech.SharedKernel;
using SachkovTech.SharedKernel.ValueObjects;
using SachkovTech.SharedKernel.ValueObjects.Ids;
using SharedKernel;

namespace SachkovTech.Issues.Application.Interfaces;

public interface IIssuesRepository
{
    Task<Guid> Add(Issue issue, CancellationToken cancellationToken = default);

    Guid Save(Issue issue, CancellationToken cancellationToken = default);

    Guid Delete(Issue issue);

    Task<Result<Issue, Error>> GetById(
        IssueId issueId,
        bool includeDeletedOption = false,
        CancellationToken cancellationToken = default);

    Task<Result<Issue, Error>> GetByTitle(Title title, CancellationToken cancellationToken = default);
}