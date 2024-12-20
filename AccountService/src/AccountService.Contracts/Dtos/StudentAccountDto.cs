namespace AccountService.Contracts.Dtos;

public class StudentAccountDto
{
    public Guid Id { get; init; }

    public Guid UserId { get; init; }

    public DateTime DateStartedStudying { get; init; }
}