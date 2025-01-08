using UserProgressService.Application.Dtos;

namespace UserProgressService.API.Controllers.Requests;

public record CreateAchievementForLessonRequest(
    Guid IconId,
    string Name,
    string Description,
    LessonConditionDto LessonCondition,
    int Experience);