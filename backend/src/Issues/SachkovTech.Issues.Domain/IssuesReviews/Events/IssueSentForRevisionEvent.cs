using SachkovTech.SharedKernel;
using SachkovTech.SharedKernel.ValueObjects.Ids;
using SharedKernel;

namespace SachkovTech.Issues.Domain.IssuesReviews.Events;

public record IssueSentForRevisionEvent(UserIssueId UserIssueId) : IDomainEvent;