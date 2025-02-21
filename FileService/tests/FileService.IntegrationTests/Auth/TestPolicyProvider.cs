using Microsoft.AspNetCore.Authorization;
using SachkovTech.Framework.Authorization;

namespace FileService.IntegrationTests.Auth;

public class TestPolicyProvider : IAuthorizationPolicyProvider
{
    public Task<AuthorizationPolicy?> GetPolicyAsync(string policyName)
    {
        if (string.IsNullOrEmpty(policyName))
        {
            return Task.FromResult<AuthorizationPolicy?>(null);
        }

        var policy = new AuthorizationPolicyBuilder("Test")
            .AddRequirements(new PermissionAttribute(policyName))
            .Build();

        return Task.FromResult<AuthorizationPolicy?>(policy);
    }

    public Task<AuthorizationPolicy> GetDefaultPolicyAsync() =>
        Task.FromResult(new AuthorizationPolicyBuilder("Test")
            .Build());

    public Task<AuthorizationPolicy?> GetFallbackPolicyAsync() =>
        Task.FromResult<AuthorizationPolicy?>(null);
}