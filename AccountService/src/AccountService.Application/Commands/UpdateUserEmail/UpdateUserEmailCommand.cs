using SachkovTech.Core.Abstractions;

namespace ProjectTemplate.Application.Commands.UpdateUserEmail;

public record UpdateUserEmailCommand(Guid UserId, string Email) : ICommand;