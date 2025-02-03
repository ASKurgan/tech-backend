using AccountService.Contracts.Dtos;

namespace AccountService.Contracts.Requests;

public record UpdateUserSocialNetworksRequest(IEnumerable<SocialNetworkDto> SocialNetworks);