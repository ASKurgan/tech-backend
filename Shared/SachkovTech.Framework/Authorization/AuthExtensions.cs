using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using SachkovTech.Core.RsaKeys;
using SachkovTech.Framework.Http;

namespace SachkovTech.Framework.Authorization;

public static class AuthExtensions
{
    public static IServiceCollection AddAuthServices(
        this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<AuthOptions>(configuration.GetSection(AuthOptions.AUTH));

        var authOptions = configuration.GetSection(AuthOptions.AUTH).Get<AuthOptions>()
                          ?? throw new ApplicationException("Missing auth configuration");

        var rsaKeyProvider = new RsaKeyProvider(authOptions);

        services.AddSingleton<IRsaKeyProvider>(rsaKeyProvider);
        services.AddSingleton<IAuthorizationHandler, PermissionRequirementHandler>();
        services.AddSingleton<IAuthorizationHandler, SecretKeyRequirementHandler>();
        services.AddSingleton<IAuthorizationPolicyProvider, PermissionPolicyProvider>();

        services.AddHttpContextAccessor();
        services.AddScoped<UserScopedData>();

        services.AddTransient<HttpTrackerHandler>();

        services
            .AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultSignInScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                var rsaKey = rsaKeyProvider.GetPublicRsa();
                var key = new RsaSecurityKey(rsaKey);

                options.TokenValidationParameters = TokenValidationParametersFactory.CreateWithLifeTime(key);
            })
            .AddSecretKey(authOptions.SecretKey);

        services.AddAuthorization();
        return services;
    }

    private static AuthenticationBuilder AddSecretKey(
        this AuthenticationBuilder builder, string key)
    {
        builder.AddScheme<SecretKeyAuthenticationOptions, SecretKeyAuthenticationHandler>("SecretKey", options =>
        {
            options.ExpectedKey = key;
        });

        return builder;
    }
}