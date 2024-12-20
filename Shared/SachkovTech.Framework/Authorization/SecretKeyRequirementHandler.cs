using Microsoft.AspNetCore.Authorization;

namespace SachkovTech.Framework.Authorization;

public class SecretKeyRequirementHandler : AuthorizationHandler<PermissionAttribute>
{
    protected override Task HandleRequirementAsync(
        AuthorizationHandlerContext context,
        PermissionAttribute permission)
    {
        if (context.User.HasClaim(c => c is { Type: "IsService", Value: "true" }))
        {
            context.Succeed(permission);
        }

        return Task.CompletedTask;
    }
}