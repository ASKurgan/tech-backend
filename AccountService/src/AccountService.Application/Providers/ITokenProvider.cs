using System.Security.Claims;
using CSharpFunctionalExtensions;
using ProjectTemplate.Application.Models;
using ProjectTemplate.Domain;
using SharedKernel;

namespace ProjectTemplate.Application.Providers;

public interface ITokenProvider
{
    Task<JwtTokenResult> GenerateAccessToken(User user, CancellationToken cancellationToken);

    Task<Guid> GenerateRefreshToken(User user, CancellationToken cancellationToken = default);

    Task<Result<IReadOnlyList<Claim>, Error>> GetUserClaims(
        string jwtToken, CancellationToken cancellationToken);
}