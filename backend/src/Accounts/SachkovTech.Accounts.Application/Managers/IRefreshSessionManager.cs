using CSharpFunctionalExtensions;
using SachkovTech.Accounts.Domain;
using SharedKernel;

namespace SachkovTech.Accounts.Application.Managers;

public interface IRefreshSessionManager
{
    Task<Result<RefreshSession, Error>> GetByRefreshToken(
        Guid refreshToken, CancellationToken cancellationToken);

    void Delete(RefreshSession refreshSession);
}