namespace UserProgressService.Application.Dtos;

public class IssueConditionDto : ConditionDto
{
    public int? Attempts { get; init; }
    public int? IssueCount { get; init; }
}