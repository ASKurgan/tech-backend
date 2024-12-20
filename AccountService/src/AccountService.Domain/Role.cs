using Microsoft.AspNetCore.Identity;

namespace ProjectTemplate.Domain;

public class Role : IdentityRole<Guid>
{
    public List<RolePermission> RolePermissions { get; set; } = [];
}