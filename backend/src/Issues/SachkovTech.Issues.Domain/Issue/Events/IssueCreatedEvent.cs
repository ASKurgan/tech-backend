using SachkovTech.SharedKernel;
using SachkovTech.SharedKernel.ValueObjects.Ids;
using SharedKernel;

namespace SachkovTech.Issues.Domain.Issue.Events;

public record IssueCreatedEvent(IssueId IssueId, ModuleId ModuleId) : IDomainEvent;