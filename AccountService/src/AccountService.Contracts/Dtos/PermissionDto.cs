namespace AccountService.Contracts.Dtos;

public class PermissionDto
{
    public Guid Id { get; init; }

    public string Code { get; init; } = string.Empty;
}