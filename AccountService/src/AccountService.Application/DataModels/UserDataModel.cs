using AccountService.Contracts.Dtos;

namespace ProjectTemplate.Application.DataModels;

public class UserDataModel
{
    public Guid Id { get; init; }

    public string? UserName { get; init; } = string.Empty;

    public string? FirstName { get; init; } = string.Empty;

    public string? SecondName { get; init; } = string.Empty;

    public string? ThirdName { get; init; } = string.Empty;

    public string Email { get; init; } = string.Empty;

    public string? PhoneNumber { get; set; } = string.Empty;

    public DateTime RegistrationDate { get; init; }

    public IReadOnlyList<SocialNetworkDto> SocialNetworks { get; init; } = [];

    public StudentAccountDto? StudentAccount { get; init; }

    public SupportAccountDto? SupportAccount { get; init; }

    public AdminAccountDto? AdminAccount { get; init; }

    public List<RoleDto> Roles { get; init; } = [];

    public List<UserRolesDataModel> UserRoles { get; init; } = null!;
}