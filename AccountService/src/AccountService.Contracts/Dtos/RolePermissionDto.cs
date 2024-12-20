namespace AccountService.Contracts.Dtos;

public class RolePermissionDto
{
    public Guid RoleId { get; init; }

    public RoleDto Role { get; init; } = default!;

    public Guid PermissionId { get; init; }

    public PermissionDto Permission { get; init; } = default!;
}