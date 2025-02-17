using AccountService.Domain;
using CSharpFunctionalExtensions;
using SharedKernel;

namespace AccountService.Application.Managers;

public interface IRefreshSessionManager
{
    Task<Result<RefreshSession, Error>> GetByRefreshToken(
        Guid refreshToken, CancellationToken cancellationToken);

    void Delete(RefreshSession refreshSession);
}