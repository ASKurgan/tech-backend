using SachkovTech.Core.Abstractions;

namespace ProjectTemplate.Application.Commands.Login;

public record LoginCommand(string Email, string Password) : ICommand;