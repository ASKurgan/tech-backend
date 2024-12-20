using SachkovTech.Core.Abstractions;

namespace ProjectTemplate.Application.Commands.Register;

public record RegisterUserCommand(
    string Email, 
    string UserName, 
    string Password) : ICommand;