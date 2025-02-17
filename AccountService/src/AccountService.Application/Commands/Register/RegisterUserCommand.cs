using SachkovTech.Core.Abstractions;

namespace AccountService.Application.Commands.Register;

public record RegisterUserCommand(
    string Email,
    string UserName,
    string Password) : ICommand;