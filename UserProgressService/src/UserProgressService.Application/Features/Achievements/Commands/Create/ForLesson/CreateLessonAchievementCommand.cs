using SachkovTech.Core.Abstractions;
using UserProgressService.Application.Dtos;

namespace UserProgressService.Application.Features.Achievements.Commands.Create.ForLesson;

public record CreateLessonAchievementCommand(
    Guid IconId,
    string Name,
    string Description,
    LessonConditionDto LessonCondition,
    int Experience) : ICommand;