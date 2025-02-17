using SachkovTech.Core.Abstractions;

namespace AccountService.Application.Commands.RefreshTokens;

public record RefreshTokensCommand(Guid RefreshToken) : ICommand;