namespace SachkovTech.Issues.Contracts.Messaging;

public record IssueSentOnReviewEvent(Guid UserId, Guid UserIssueId, Guid IssueId);