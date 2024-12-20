using Microsoft.IdentityModel.Tokens;

namespace SachkovTech.Framework.Authorization;

public static class TokenValidationParametersFactory
{
    public static TokenValidationParameters CreateWithLifeTime(RsaSecurityKey key) =>
        new()
        {
            IssuerSigningKey = key,
            ValidateIssuer = false,
            ValidateAudience = false,
            ValidateLifetime = true,
            ClockSkew = TimeSpan.Zero,
        };

    public static TokenValidationParameters CreateWithoutLifeTime(RsaSecurityKey key) =>
        new()
        {
            IssuerSigningKey = key,
            ValidateIssuer = false,
            ValidateAudience = false,
            ValidateLifetime = false,
            ClockSkew = TimeSpan.Zero,
        };
}