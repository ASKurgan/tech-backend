using SachkovTech.Core.Abstractions;

namespace ProjectTemplate.Application.Commands.Logout;

public record LogoutCommand(Guid RefreshToken) : ICommand;