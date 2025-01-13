using SachkovTech.Core.Abstractions;

namespace SachkovTech.Issues.Application.Features.IssueSolving.Commands.CompleteIssue;

public record CompleteIssueCommand(
    Guid UserIssueId) : ICommand;