using SachkovTech.Accounts.Application.Managers;
using SachkovTech.Accounts.Contracts;

namespace SachkovTech.Accounts.Presentation;

public class AccountsContract : IAccountsContract
{
    private readonly IPermissionManager _permissionManager;

    public AccountsContract(IPermissionManager permissionManager)
    {
        _permissionManager = permissionManager;
    }

    public async Task<HashSet<string>> GetUserPermissionCodes(Guid userId)
    {
        return await _permissionManager.GetUserPermissionCodes(userId);
    }
}