using SachkovTech.Core.Abstractions;

namespace SachkovTech.Issues.Application.Features.Issue.Commands.RestoreIssue;

public record RestoreIssueCommand(Guid IssueId) : ICommand;