using AccountService.Contracts.Dtos;

namespace ProjectTemplate.Application.DataModels;

public class UserRolesDataModel
{
    public Guid RoleId { get; init; }
    public RoleDto Role { get; init; } = default!;

    public Guid UserId { get; init; }
    public UserDataModel User { get; init; } = default!;
}