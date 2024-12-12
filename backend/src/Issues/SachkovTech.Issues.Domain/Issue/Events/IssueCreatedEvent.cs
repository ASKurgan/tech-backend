using SachkovTech.SharedKernel;
using SachkovTech.SharedKernel.ValueObjects.Ids;

namespace SachkovTech.Issues.Domain.Issue.Events;

public record IssueCreatedEvent(IssueId IssueId, ModuleId ModuleId) : IDomainEvent;