using SachkovTech.SharedKernel;

namespace SachkovTech.Issues.Domain.Issue.Events;

public record IssueDeletedEvent(Guid ModuleId, Guid IssueId) : IDomainEvent;