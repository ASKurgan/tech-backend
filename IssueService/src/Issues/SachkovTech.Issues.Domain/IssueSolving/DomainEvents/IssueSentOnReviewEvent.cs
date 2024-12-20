using SachkovTech.Issues.Domain.ValueObjects;
using SachkovTech.Issues.Domain.ValueObjects.Ids;
using SharedKernel;

namespace SachkovTech.Issues.Domain.IssueSolving.DomainEvents;

public record IssueSentOnReviewEvent(
    UserIssueId UserIssueId,
    Guid UserId,
    PullRequestUrl PullRequestUrl) : IDomainEvent;