namespace UserProgressService.Application.Dtos;

public abstract class ConditionDto
{
    public TimeSpan? TimeToComplete { get; init; }
    public string Difficulty { get; init; } = string.Empty;
}