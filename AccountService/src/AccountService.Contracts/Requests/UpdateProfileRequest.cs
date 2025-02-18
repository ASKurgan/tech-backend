using AccountService.Contracts.Dtos;

namespace AccountService.Contracts.Requests;

public record UpdateProfileRequest(
    string UserName,
    string? FirstName,
    string? SecondName,
    string? ThirdName,
    IEnumerable<SocialNetworkDto> Socials);