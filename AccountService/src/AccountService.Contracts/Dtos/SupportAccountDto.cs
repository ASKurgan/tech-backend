namespace AccountService.Contracts.Dtos;

public class SupportAccountDto
{
    public Guid Id { get; init; }

    public Guid UserId { get; init; }

    public string AboutSelf { get; init; } = default!;
}