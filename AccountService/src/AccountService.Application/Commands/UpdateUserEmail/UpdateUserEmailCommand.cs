using SachkovTech.Core.Abstractions;

namespace AccountService.Application.Commands.UpdateUserEmail;

public record UpdateUserEmailCommand(Guid UserId, string Email) : ICommand;