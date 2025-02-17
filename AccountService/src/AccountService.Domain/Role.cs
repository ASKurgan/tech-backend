using Microsoft.AspNetCore.Identity;

namespace AccountService.Domain;

public class Role : IdentityRole<Guid>
{
    public List<RolePermission> RolePermissions { get; set; } = [];
}