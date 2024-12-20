using SachkovTech.Core.Abstractions;

namespace SachkovTech.Issues.Application.Features.Issue.Commands.DeleteIssue;

public record DeleteIssueCommand(Guid IssueId) : ICommand;
