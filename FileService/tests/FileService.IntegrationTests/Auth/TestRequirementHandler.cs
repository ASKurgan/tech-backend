using Microsoft.AspNetCore.Authorization;
using SachkovTech.Framework.Authorization;

namespace FileService.IntegrationTests.Auth;

public class TestRequirementHandler : AuthorizationHandler<PermissionAttribute>
{
    protected override Task HandleRequirementAsync(
        AuthorizationHandlerContext context,
        PermissionAttribute permission)
    {
        context.Succeed(permission);
        return Task.CompletedTask;
    }
}