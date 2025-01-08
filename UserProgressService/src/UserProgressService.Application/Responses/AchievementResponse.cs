using UserProgressService.Application.Dtos;

namespace UserProgressService.Application.Responses;

public class AchievementResponse
{
    public Guid Id { get; init; }
    public Guid IconId { get; init; }
    public string Name { get; init; } = string.Empty;
    public string Description { get; init; } = string.Empty;
    public DateOnly CreatedDate { get; init; }
    public int Experience { get; init; }
    public ConditionDto Condition { get; init; } = null!;
}