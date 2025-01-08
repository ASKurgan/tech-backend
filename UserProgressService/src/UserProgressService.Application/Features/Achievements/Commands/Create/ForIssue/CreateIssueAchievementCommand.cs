using SachkovTech.Core.Abstractions;
using UserProgressService.Application.Dtos;

namespace UserProgressService.Application.Features.Achievements.Commands.Create.ForIssue;

public record CreateIssueAchievementCommand(
    Guid IconId,
    string Name,
    string Description,
    IssueConditionDto IssueCondition,
    int Experience) : ICommand;