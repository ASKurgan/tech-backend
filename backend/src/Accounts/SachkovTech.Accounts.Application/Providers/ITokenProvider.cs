using System.Security.Claims;
using CSharpFunctionalExtensions;
using SachkovTech.Accounts.Application.Models;
using SachkovTech.Accounts.Domain;
using SachkovTech.SharedKernel;
using SharedKernel;

namespace SachkovTech.Accounts.Application.Providers;

public interface ITokenProvider
{
    Task<JwtTokenResult> GenerateAccessToken(User user, CancellationToken cancellationToken);
    Task<Guid> GenerateRefreshToken(User user, CancellationToken cancellationToken = default);

    Task<Result<IReadOnlyList<Claim>, Error>> GetUserClaims(
        string jwtToken, CancellationToken cancellationToken);
}