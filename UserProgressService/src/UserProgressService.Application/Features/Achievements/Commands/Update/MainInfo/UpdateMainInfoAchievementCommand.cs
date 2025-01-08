using SachkovTech.Core.Abstractions;
using UserProgressService.Domain.ValueObjects.Ids;

namespace UserProgressService.Application.Features.Achievements.Commands.Update.MainInfo;

public record UpdateMainInfoAchievementCommand(
    AchievementId Id,
    Guid IconId,
    string Name,
    string Description,
    int Experience) : ICommand;