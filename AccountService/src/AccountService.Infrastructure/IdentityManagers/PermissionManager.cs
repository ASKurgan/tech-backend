using AccountService.Application.Managers;
using AccountService.Domain;
using AccountService.Infrastructure.DbContexts;
using Microsoft.EntityFrameworkCore;

namespace AccountService.Infrastructure.IdentityManagers;

public class PermissionManager(AccountsDbContext accountsContext) : IPermissionManager
{
    public async Task<Permission?> FindByCode(string code)
        => await accountsContext.Permissions.FirstOrDefaultAsync(p => p.Code == code);

    public async Task AddRangeIfExist(
        IEnumerable<string> permissions, CancellationToken cancellationToken = default)
    {
        foreach (string permissionCode in permissions)
        {
            bool isPermissionExist = await accountsContext.Permissions
                .AnyAsync(p => p.Code == permissionCode, cancellationToken);

            if (isPermissionExist)
                continue;

            await accountsContext.Permissions.AddAsync(
                new Permission
                {
                    Code = permissionCode,
                }, cancellationToken);
        }

        await accountsContext.SaveChangesAsync(cancellationToken);
    }

    public async Task<HashSet<string>> GetUserPermissionCodes(
        Guid userId, CancellationToken cancellationToken = default)
    {
        var permissions = await accountsContext.ReadUsers
            .Include(u => u.Roles)
            .Where(u => u.Id == userId)
            .SelectMany(u => u.Roles)
            .SelectMany(r => r.RolePermissions)
            .Select(rp => rp.Permission!.Code)
            .ToListAsync(cancellationToken);

        return permissions.ToHashSet();
    }
}