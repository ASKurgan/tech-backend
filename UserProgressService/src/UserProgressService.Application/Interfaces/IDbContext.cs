using UserProgressService.Domain.Achievements;
using UserProgressService.Domain.Progress;

namespace UserProgressService.Application.Interfaces;

public interface IDbContext
{
    public IQueryable<UserProgress> UserProgressesQuery { get; }
    public IQueryable<Achievement> AchievementsQuery { get; }
}