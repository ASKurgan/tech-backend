using SachkovTech.Core.Abstractions;
using UserProgressService.Application.Dtos;
using UserProgressService.Domain.ValueObjects.Ids;

namespace UserProgressService.Application.Features.Achievements.Commands.Update.ForIssue;

public record UpdateIssueAchievementConditionCommand(
    AchievementId Id,
    IssueConditionDto IssueCondition) : ICommand;