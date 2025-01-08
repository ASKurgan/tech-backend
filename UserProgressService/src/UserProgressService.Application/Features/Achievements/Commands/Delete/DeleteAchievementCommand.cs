using SachkovTech.Core.Abstractions;
using UserProgressService.Domain.ValueObjects.Ids;

namespace UserProgressService.Application.Features.Achievements.Commands.Delete;

public record DeleteAchievementCommand(AchievementId Id) : ICommand;