using SachkovTech.Core.Abstractions;

namespace SachkovTech.Issues.Application.Features.IssueSolving.Commands.TakeOnWork;

public record TakeOnWorkCommand(Guid UserId, Guid IssueId, Guid ModuleId) : ICommand;