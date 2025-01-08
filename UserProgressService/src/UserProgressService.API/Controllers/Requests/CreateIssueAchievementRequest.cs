using UserProgressService.Application.Dtos;

namespace UserProgressService.API.Controllers.Requests;

public record CreateIssueAchievementRequest(
    Guid IconId,
    string Name,
    string Description,
    IssueConditionDto IssueCondition,
    int Experience);