namespace SachkovTech.Accounts.Contracts.Dtos;

public record SocialNetworkDto
{
    public required string Name { get; init; }

    public required string Link { get; init; }
}