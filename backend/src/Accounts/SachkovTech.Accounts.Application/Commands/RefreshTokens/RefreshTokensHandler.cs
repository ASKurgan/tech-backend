using CSharpFunctionalExtensions;
using Microsoft.Extensions.DependencyInjection;
using SachkovTech.Accounts.Application.Managers;
using SachkovTech.Accounts.Application.Providers;
using SachkovTech.Accounts.Contracts.Responses;
using SachkovTech.Core.Abstractions;
using SachkovTech.SharedKernel;
using SharedKernel;

namespace SachkovTech.Accounts.Application.Commands.RefreshTokens;

public class RefreshTokensHandler : ICommandHandler<LoginResponse, RefreshTokensCommand>
{
    private readonly IRefreshSessionManager _refreshSessionManager;
    private readonly ITokenProvider _tokenProvider;
    private readonly IUnitOfWork _unitOfWork;

    public RefreshTokensHandler(
        IRefreshSessionManager refreshSessionManager,
        ITokenProvider tokenProvider,
        [FromKeyedServices(Modules.Accounts)] IUnitOfWork unitOfWork)
    {
        _refreshSessionManager = refreshSessionManager;
        _tokenProvider = tokenProvider;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<LoginResponse, ErrorList>> Handle(
        RefreshTokensCommand command, CancellationToken cancellationToken = default)
    {
        var oldRefreshSession = await _refreshSessionManager
            .GetByRefreshToken(command.RefreshToken, cancellationToken);

        if (oldRefreshSession.IsFailure)
            return oldRefreshSession.Error.ToErrorList();

        if (oldRefreshSession.Value.ExpiresIn < DateTime.UtcNow)
        {
            return Errors.Tokens.ExpiredToken().ToErrorList();
        }

        _refreshSessionManager.Delete(oldRefreshSession.Value);
        await _unitOfWork.SaveChanges(cancellationToken);

        var accessToken = await _tokenProvider
            .GenerateAccessToken(oldRefreshSession.Value.User, cancellationToken);

        var refreshToken = await _tokenProvider
            .GenerateRefreshToken(oldRefreshSession.Value.User, cancellationToken);

        var roles = oldRefreshSession.Value.User.Roles
            .Where(r => !string.IsNullOrEmpty(r.Name))
            .Select(r => r.Name!.ToLower());

        return new LoginResponse(
            accessToken.AccessToken,
            refreshToken,
            oldRefreshSession.Value.User.Id,
            roles);
    }
}