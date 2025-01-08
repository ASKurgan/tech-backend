using CSharpFunctionalExtensions;
using SharedKernel;
using UserProgressService.Domain.ValueObjects.Ids;

namespace UserProgressService.Application.Interfaces;

public interface IAchievementRepository
{
    public Task<Guid> Add(Domain.Achievements.Achievement achievement, CancellationToken cancellationToken = default);

    public Guid Delete(Domain.Achievements.Achievement achievement);

    public Task<Result<Domain.Achievements.Achievement, Error>> GetById(
        AchievementId achievementId,
        CancellationToken cancellationToken = default);
}