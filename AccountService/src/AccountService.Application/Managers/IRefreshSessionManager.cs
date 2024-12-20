using CSharpFunctionalExtensions;
using ProjectTemplate.Domain;
using SharedKernel;

namespace ProjectTemplate.Application.Managers;

public interface IRefreshSessionManager
{
    Task<Result<RefreshSession, Error>> GetByRefreshToken(
        Guid refreshToken, CancellationToken cancellationToken);

    void Delete(RefreshSession refreshSession);
}