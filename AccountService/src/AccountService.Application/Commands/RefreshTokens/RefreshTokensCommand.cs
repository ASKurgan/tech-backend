using SachkovTech.Core.Abstractions;

namespace ProjectTemplate.Application.Commands.RefreshTokens;

public record RefreshTokensCommand(Guid RefreshToken) : ICommand;