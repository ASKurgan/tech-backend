using SachkovTech.Core.Abstractions;

namespace AccountService.Application.Commands.UpdateEmail;

public record UpdateEmailCommand(Guid UserId, string Email) : ICommand;