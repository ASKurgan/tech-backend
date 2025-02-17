using SachkovTech.Core.Abstractions;

namespace AccountService.Application.Commands.Logout;

public record LogoutCommand(Guid RefreshToken) : ICommand;