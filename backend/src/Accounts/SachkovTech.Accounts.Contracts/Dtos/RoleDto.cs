namespace SachkovTech.Accounts.Contracts.Dtos;

public class RoleDto
{
    public Guid Id { get; init; }

    public string Name { get; init; } = default!;

    public List<PermissionDto> Permissions { get; init; } = default!;
}