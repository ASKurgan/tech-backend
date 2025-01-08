using UserProgressService.Domain.Achievements;
using UserProgressService.Domain.DomainServices;
using UserProgressService.Domain.Progress;
using UserProgressService.Domain.ValueObjects;
using UserProgressService.Domain.ValueObjects.Conditions;
using UserProgressService.Domain.ValueObjects.Ids;

namespace UserProgressService.UnitTests.Domain;

public class CheckConditionsServiceTests
{
    private readonly CheckConditionsService _checkConditionsService;

    public CheckConditionsServiceTests()
    {
        _checkConditionsService = new CheckConditionsService();
    }

    [Fact]
    public void CheckConditions_AchievementIssueCondition_ShouldAddAchievement()
    {
        // Arrange
        var userProgress = new UserProgress(Guid.NewGuid(), Level.Create(1).Value);

        var issueId = Guid.NewGuid();

        userProgress.AddIssueProgress(new IssueProgress(
            Guid.NewGuid(),
            Guid.NewGuid(),
            issueId,
            1,
            TimeSpan.FromMinutes(30),
            Difficulty.Create(Difficulty.Medium).Value));

        var condition = IssueCondition.Create(
            TimeSpan.FromMinutes(60),
            Difficulty.Create(Difficulty.Medium).Value,
            1,
            1).Value;

        var achievement = new Achievement(
            Guid.NewGuid(),
            Guid.NewGuid(),
            "",
            "",
            condition,
            0);

        var achievements = new List<Achievement> { achievement };

        // Act
        _checkConditionsService.CheckConditions(achievements, userProgress);

        // Assert
        Assert.Contains(userProgress.Achievements, a => a == achievement.Id);
    }

    [Fact]
    public void CheckConditions_AchievementIssueCondition_ShouldNotAddAchievement()
    {
        // Arrange
        var userProgress = new UserProgress(Guid.NewGuid(), Level.Create(1).Value);

        var issueId = Guid.NewGuid();
        userProgress.AddIssueProgress(new IssueProgress(
            Guid.NewGuid(),
            Guid.NewGuid(),
            issueId,
            1,
            TimeSpan.FromMinutes(120),
            Difficulty.Create(Difficulty.Medium).Value));

        var condition = IssueCondition.Create(
            TimeSpan.FromMinutes(60),
            Difficulty.Create(Difficulty.Medium).Value,
            1,
            1).Value;

        var achievement = new Achievement(
            Guid.NewGuid(),
            Guid.NewGuid(),
            "",
            "",
            condition,
            0);

        var achievements = new List<Achievement> { achievement };

        // Act
        _checkConditionsService.CheckConditions(achievements, userProgress);

        // Assert
        Assert.DoesNotContain(userProgress.Achievements, a => a == achievement.Id);
    }

    [Fact]
    public void CheckConditions_AchievementLessonCondition_ShouldAddAchievement()
    {
        // Arrange
        var userProgress = new UserProgress(Guid.NewGuid(), Level.Create(1).Value);

        var lessonId = Guid.NewGuid();
        userProgress.AddLessonProgress(new LessonProgress(
            LessonProgressId.NewId(),
            Guid.NewGuid(),
            lessonId,
            true)); // Завершённый урок

        var condition = LessonCondition.Create(
            TimeSpan.FromMinutes(30),
            Difficulty.Create(Difficulty.Easy).Value,
            1).Value; // Условие: посмотреть 1 урок

        var achievement = new Achievement(
            Guid.NewGuid(),
            Guid.NewGuid(),
            "",
            "",
            condition,
            0);

        var achievements = new List<Achievement> { achievement };

        // Act
        _checkConditionsService.CheckConditions(achievements, userProgress);

        // Assert
        Assert.Contains(userProgress.Achievements, a => a == achievement.Id);
    }

    [Fact]
    public void CheckConditions_AchievementLessonCondition_ShouldNotAddAchievement()
    {
        // Arrange
        var userProgress = new UserProgress(Guid.NewGuid(), Level.Create(1).Value);

        var lessonId = Guid.NewGuid();
        userProgress.AddLessonProgress(new LessonProgress(
            LessonProgressId.NewId(),
            Guid.NewGuid(),
            lessonId,
            false)); // Не завершённый урок

        var condition = LessonCondition.Create(
            TimeSpan.FromMinutes(30),
            Difficulty.Create(Difficulty.Easy).Value,
            1).Value; // Условие: посмотреть 1 урок

        var achievement = new Achievement(
            Guid.NewGuid(),
            Guid.NewGuid(),
            "",
            "",
            condition,
            0);

        var achievements = new List<Achievement> { achievement };

        // Act
        _checkConditionsService.CheckConditions(achievements, userProgress);

        // Assert
        Assert.DoesNotContain(userProgress.Achievements, a => a == achievement.Id);
    }

    [Fact]
    public void CheckConditions_AchievementCondition_WithMultipleConditions_ShouldAddAchievement()
    {
        // Arrange
        var userProgress = new UserProgress(Guid.NewGuid(), Level.Create(1).Value);

        var issueId = Guid.NewGuid();

        userProgress.AddIssueProgress(new IssueProgress(
            Guid.NewGuid(),
            Guid.NewGuid(),
            issueId,
            1,
            TimeSpan.FromMinutes(30),
            Difficulty.Create(Difficulty.Medium).Value)); // Добавляем завершённую задачу

        var lessonId = Guid.NewGuid();
        userProgress.AddLessonProgress(new LessonProgress(
            LessonProgressId.NewId(),
            Guid.NewGuid(),
            lessonId,
            true)); // Завершённый урок

        var issueCondition = IssueCondition.Create(
            TimeSpan.FromMinutes(60),
            Difficulty.Create(Difficulty.Medium).Value,
            1,
            1).Value; // Условие: выполнить 1 задачу за 60 минут

        var lessonCondition = LessonCondition.Create(
            TimeSpan.FromMinutes(30),
            Difficulty.Create(Difficulty.Easy).Value,
            1).Value; // Условие: посмотреть 1 урок

        var lessonAchievement = new Achievement(
            Guid.NewGuid(),
            Guid.NewGuid(),
            "",
            "",
            lessonCondition,
            0);

        var issueAchievement = new Achievement(
            Guid.NewGuid(),
            Guid.NewGuid(),
            "",
            "",
            issueCondition,
            0);

        var achievements = new List<Achievement> { lessonAchievement, issueAchievement };

        // Act
        _checkConditionsService.CheckConditions(achievements, userProgress);

        // Assert
        Assert.Contains(userProgress.Achievements, a => a == lessonAchievement.Id);
        Assert.Contains(userProgress.Achievements, a => a == issueAchievement.Id);
    }
}