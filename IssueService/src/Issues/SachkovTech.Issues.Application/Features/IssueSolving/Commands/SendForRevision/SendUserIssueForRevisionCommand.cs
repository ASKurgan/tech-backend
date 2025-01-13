using SachkovTech.Core.Abstractions;

namespace SachkovTech.Issues.Application.Features.IssueSolving.Commands.SendForRevision;

public record SendUserIssueForRevisionCommand(
    Guid UserIssueId) : ICommand;