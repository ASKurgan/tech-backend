using CSharpFunctionalExtensions;
using UserProgressService.Domain.Enums;
using UserProgressService.Domain.ValueObjects;
using UserProgressService.Domain.ValueObjects.Ids;

namespace UserProgressService.Domain.Progress;

public class UserProgress : Entity<UserProgressId>
{
    private readonly List<AchievementId> _achievements = [];
    private readonly List<IssueProgress> _issueProgresses = [];
    private readonly List<LessonProgress> _lessonProgresses = [];

    // EF Core конструктор
    private UserProgress(UserProgressId id) : base(id)
    {
    }

    public UserProgress(UserProgressId id, Level level, int experience) : base(id)
    {
        Level = level;
        Experience = experience;
    }

    public Guid UserId { get; private set; }
    public Level Level { get; private set; }
    public int Experience { get; private set; }
    public DateOnly UserActivityDate { get; private set; }
    public IReadOnlyList<AchievementId> Achievements => _achievements;
    public IReadOnlyList<IssueProgress> IssueProgresses => _issueProgresses;
    public IReadOnlyList<LessonProgress> LessonProgresses => _lessonProgresses;

    public void GainExperience(int experience)
    {
        var newExperience = Level.CurrentExperience + experience;
        Level = new Level(newExperience);
    }

    public void AddIssueProgress(IssueProgress issueProgress)
    {
        _issueProgresses.Add(issueProgress);
    }

    public void AddLessonProgress(LessonProgress lessonProgress)
    {
        _lessonProgresses.Add(lessonProgress);
    }

    public void AddAchievement(AchievementId achievementId)
    {
        if (!_achievements.Contains(achievementId))
            _achievements.Add(achievementId);
    }

    // Доменные запросы для удобства проверки условий:
    public int GetCompletedLessonCount() => _lessonProgresses.Count(lp => lp.IsCompleted);

    private bool MatchesFilters(IssueProgress progress, Difficulty difficulty, TimeSpan? maxTime, int? maxAttempts)
    {
        if (progress.Difficulty != difficulty)
            return false;

        if (maxTime.HasValue && progress.ExecutionTime > maxTime.Value)
            return false;

        if (maxAttempts.HasValue && progress.TryCount > maxAttempts.Value)
            return false;

        return true;
    }

    public List<IssueProgress> GetIssues(Difficulty difficulty, TimeSpan? maxTime = null, int? maxAttempts = null)
    {
        return _issueProgresses
            .Where(i => MatchesFilters(i, difficulty, maxTime, maxAttempts))
            .OrderBy(i => i.StartedDate)
            .ToList();
    }


    public bool HasAchievement(AchievementId id) => _achievements.Contains(id);
}