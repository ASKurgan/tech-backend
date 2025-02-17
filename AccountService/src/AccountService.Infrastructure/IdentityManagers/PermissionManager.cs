using AccountService.Application.Managers;
using AccountService.Domain;
using AccountService.Infrastructure.DbContexts;
using Microsoft.EntityFrameworkCore;

namespace AccountService.Infrastructure.IdentityManagers;

public class PermissionManager(AccountsWriteDbContext accountsWriteContext) : IPermissionManager
{
    public async Task<Permission?> FindByCode(string code)
        => await accountsWriteContext.Permissions.FirstOrDefaultAsync(p => p.Code == code);

    public async Task AddRangeIfExist(
        IEnumerable<string> permissions, CancellationToken cancellationToken = default)
    {
        foreach (string permissionCode in permissions)
        {
            bool isPermissionExist = await accountsWriteContext.Permissions
                .AnyAsync(p => p.Code == permissionCode, cancellationToken);

            if (isPermissionExist)
                continue;

            await accountsWriteContext.Permissions.AddAsync(
                new Permission
                {
                    Code = permissionCode,
                }, cancellationToken);
        }

        await accountsWriteContext.SaveChangesAsync(cancellationToken);
    }

    public async Task<HashSet<string>> GetUserPermissionCodes(
        Guid userId, CancellationToken cancellationToken = default)
    {
        var permissions = await accountsWriteContext.Users
            .Include(u => u.Roles)
            .Where(u => u.Id == userId)
            .SelectMany(u => u.Roles)
            .SelectMany(r => r.RolePermissions)
            .Select(rp => rp.Permission!.Code)
            .ToListAsync(cancellationToken);

        return permissions.ToHashSet();
    }
}