using SachkovTech.Core.Abstractions;

namespace SachkovTech.Issues.Application.Features.IssueSolving.Commands.StopWorking;

public record StopWorkingCommand(Guid UserIssueId, Guid UserId) : ICommand;