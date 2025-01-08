using CSharpFunctionalExtensions;
using Microsoft.EntityFrameworkCore;
using SharedKernel;
using UserProgressService.Application.Interfaces;
using UserProgressService.Domain.Achievements;
using UserProgressService.Domain.ValueObjects.Ids;

namespace UserProgressService.Infrastructure.Repositories;

public class AchievementRepository : IAchievementRepository
{
    private readonly AppDbContext _dbContext;

    public AchievementRepository(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<Guid> Add(Achievement achievement, CancellationToken cancellationToken = default)
    {
        await _dbContext.Achievements.AddAsync(achievement, cancellationToken);
        return achievement.Id;
    }

    public Guid Delete(Achievement achievement)
    {
        _dbContext.Achievements.Remove(achievement);
        return achievement.Id;
    }

    public async Task<Result<Achievement, Error>> GetById(
        AchievementId achievementId,
        CancellationToken cancellationToken = default)
    {
        var achievement = await _dbContext.Achievements
            .FirstOrDefaultAsync(a => a.Id == achievementId, cancellationToken);

        if (achievement is null)
            return Errors.General.NotFound(achievementId);

        return achievement;
    }
}