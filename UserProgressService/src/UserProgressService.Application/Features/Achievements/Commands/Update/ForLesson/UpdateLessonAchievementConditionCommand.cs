using SachkovTech.Core.Abstractions;
using UserProgressService.Application.Dtos;
using UserProgressService.Domain.ValueObjects.Ids;

namespace UserProgressService.Application.Features.Achievements.Commands.Update.ForLesson;

public record UpdateLessonAchievementConditionCommand(
    AchievementId Id,
    LessonConditionDto LessonCondition) : ICommand;