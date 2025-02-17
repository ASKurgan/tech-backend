using System.Text.Json;
using AccountService.Application.Managers;
using AccountService.Domain;
using AccountService.Infrastructure.IdentityManagers;
using AccountService.Infrastructure.Options;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using SachkovTech.Core.Database;

namespace AccountService.Infrastructure.Seeding;

public class AccountsSeederService(
    UserManager<User> userManager,
    RoleManager<Role> roleManager,
    AccountsManager accountsManager,
    IPermissionManager permissionManager,
    RolePermissionManager rolePermissionManager,
    IOptions<AdminOptions> adminOptions,
    ILogger<AccountsSeederService> logger,
    IUnitOfWork unitOfWork)
{
    private readonly AdminOptions _adminOptions = adminOptions.Value;

    public async Task SeedAsync()
    {
        logger.LogInformation("Seeding accounts...");

        var json = await File.ReadAllTextAsync("etc/accounts.json");

        var seedData = JsonSerializer.Deserialize<RolePermissionOptions>(json)
                       ?? throw new ApplicationException("Could not deserialize role permission config.");

        await SeedPermissions(seedData);

        await SeedRoles(seedData);

        await SeedRolePermissions(seedData);

        await SeedAdminAccount();
    }

    private async Task SeedRolePermissions(RolePermissionOptions seedData)
    {
        foreach (var roleName in seedData.Roles.Keys)
        {
            var role = await roleManager.FindByNameAsync(roleName);

            var rolePermissions = seedData.Roles[roleName];

            await rolePermissionManager.AddRangeIfExist(role!.Id, rolePermissions);
        }

        logger.LogInformation("Role permissions added to database.");
    }

    private async Task SeedRoles(RolePermissionOptions seedData)
    {
        foreach (var roleName in seedData.Roles.Keys)
        {
            var role = await roleManager.FindByNameAsync(roleName);

            if (role is null)
            {
                await roleManager.CreateAsync(new Role { Name = roleName });
            }
        }

        logger.LogInformation("Roles added to database.");
    }

    private async Task SeedPermissions(RolePermissionOptions seedData)
    {
        var permissionsToAdd = seedData.Permissions.SelectMany(permissionGroup => permissionGroup.Value);

        await permissionManager.AddRangeIfExist(permissionsToAdd);

        logger.LogInformation("Permissions added to database.");
    }

    private async Task SeedAdminAccount()
    {
        var adminExists = await userManager.Users
            .FirstOrDefaultAsync(u => u.Email == _adminOptions.Email);

        if (adminExists is not null)
            return;

        var adminRole = await roleManager.FindByNameAsync(AdminAccount.ADMIN)
                        ?? throw new ApplicationException("Could not find admin role.");

        var transaction = await unitOfWork.BeginTransaction();

        try
        {
            var fullName = FullName.Create(_adminOptions.UserName, _adminOptions.UserName, _adminOptions.UserName)
                .Value;

            var adminUser = User.CreateAdmin(
                _adminOptions.UserName,
                _adminOptions.Email,
                fullName,
                adminRole);

            if (adminUser.IsFailure)
                throw new ApplicationException(adminUser.Error.Message);

            await userManager.CreateAsync(adminUser.Value, _adminOptions.Password);

            var adminAccount = new AdminAccount(adminUser.Value);

            await accountsManager.CreateAdminAccount(adminAccount);

            await unitOfWork.SaveChanges();

            transaction.Commit();

            logger.LogInformation("Admin account added to database");
        }
        catch (Exception ex)
        {
            logger.LogError("Creating admin was failed");

            transaction.Rollback();

            throw new ApplicationException(ex.Message);
        }
    }
}