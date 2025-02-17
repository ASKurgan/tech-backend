using AccountService.Contracts.Dtos;
using SachkovTech.Core.Abstractions;

namespace AccountService.Application.Commands.UpdateUserSocialNetworks;

public record UpdateUserSocialNetworksCommand(Guid UserId, IEnumerable<SocialNetworkDto> SocialNetworks) : ICommand;