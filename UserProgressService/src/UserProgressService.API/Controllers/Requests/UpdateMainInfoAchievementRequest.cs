namespace UserProgressService.API.Controllers.Requests;

public record UpdateMainInfoAchievementRequest(
    Guid IconId,
    string Name,
    string Description,
    int Experience);