using AccountService.Contracts.Dtos;

namespace AccountService.Contracts.Responses;

public record UserResponse(
    Guid Id,
    string FirstName,
    string SecondName,
    string ThirdName,
    string Email,
    DateTime RegistrationDate,
    IEnumerable<SocialNetworkDto> SocialNetworks,
    StudentAccountDto? StudentAccount,
    SupportAccountDto? SupportAccount,
    AdminAccountDto? AdminAccount,
    IEnumerable<RoleDto> Roles);