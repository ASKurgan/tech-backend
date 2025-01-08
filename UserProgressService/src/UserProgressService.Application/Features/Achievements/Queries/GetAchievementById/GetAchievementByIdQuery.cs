using SachkovTech.Core.Abstractions;
using UserProgressService.Domain.ValueObjects.Ids;

namespace UserProgressService.Application.Features.Achievements.Queries.GetAchievementById;

public record GetAchievementByIdQuery(AchievementId AchievementId) : IQuery;