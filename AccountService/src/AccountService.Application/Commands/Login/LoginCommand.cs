using SachkovTech.Core.Abstractions;

namespace AccountService.Application.Commands.Login;

public record LoginCommand(string Email, string Password) : ICommand;