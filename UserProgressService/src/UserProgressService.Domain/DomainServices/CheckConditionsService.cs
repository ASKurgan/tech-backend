using UserProgressService.Domain.Achievements;
using UserProgressService.Domain.Progress;

namespace UserProgressService.Domain.DomainServices;

public class CheckConditionsService
{
    public void CheckConditions(List<Achievement> achievements, UserProgress progress)
    {
        foreach (var achievement in achievements)
        {
            if (!progress.HasAchievement(achievement.Id) && achievement.Condition.IsSatisfiedBy(progress))
            {
                progress.AddAchievement(achievement.Id);
            }
        }
    }
}