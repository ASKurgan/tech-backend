using UserProgressService.Domain.Achievements;
using UserProgressService.Domain.DomainServices;
using UserProgressService.Domain.Enums;
using UserProgressService.Domain.Progress;
using UserProgressService.Domain.ValueObjects;
using UserProgressService.Domain.ValueObjects.Conditions;

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
        var userProgress = new UserProgress(Guid.NewGuid(), new Level(1), 0);

        var issueId = Guid.NewGuid();

        userProgress.AddIssueProgress(new IssueProgress(
            Guid.NewGuid(),
            Guid.NewGuid(),
            issueId,
            1,
            DateTime.Now,
            TimeSpan.FromMinutes(30),
            Difficulty.Medium));

        var condition = new IssueCondition(
            TimeSpan.FromMinutes(60),
            Difficulty.Medium,
            1,
            1);

        var achievement = new Achievement(
            Guid.NewGuid(),
            Guid.NewGuid(),
            "",
            "",
            DateOnly.FromDateTime(DateTime.Now),
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
        var userProgress = new UserProgress(Guid.NewGuid(), new Level(1), 10);

        var issueId = Guid.NewGuid();
        userProgress.AddIssueProgress(new IssueProgress(
            Guid.NewGuid(),
            Guid.NewGuid(),
            issueId,
            1,
            DateTime.Now,
            TimeSpan.FromMinutes(120),
            Difficulty.Medium));

        var condition = new IssueCondition(
            TimeSpan.FromMinutes(60),
            Difficulty.Medium,
            1,
            1);

        var achievement = new Achievement(
            Guid.NewGuid(),
            Guid.NewGuid(),
            "",
            "",
            DateOnly.FromDateTime(DateTime.Now),
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
        var userProgress = new UserProgress(Guid.NewGuid(), new Level(1), 10);

        var lessonId = Guid.NewGuid();
        userProgress.AddLessonProgress(new LessonProgress(
            Guid.NewGuid(),
            lessonId,
            true)); // Завершённый урок

        var condition = new LessonCondition(
            TimeSpan.FromMinutes(30),
            Difficulty.Easy,
            1); // Условие: посмотреть 1 урок

        var achievement = new Achievement(
            Guid.NewGuid(),
            Guid.NewGuid(),
            "",
            "",
            DateOnly.FromDateTime(DateTime.Now),
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
        var userProgress = new UserProgress(Guid.NewGuid(), new Level(1), 10);

        var lessonId = Guid.NewGuid();
        userProgress.AddLessonProgress(new LessonProgress(
            Guid.NewGuid(),
            lessonId,
            false)); // Не завершённый урок

        var condition = new LessonCondition(
            TimeSpan.FromMinutes(30),
            Difficulty.Easy,
            1); // Условие: посмотреть 1 урок

        var achievement = new Achievement(
            Guid.NewGuid(),
            Guid.NewGuid(),
            "",
            "",
            DateOnly.FromDateTime(DateTime.Now),
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
        var userProgress = new UserProgress(Guid.NewGuid(), new Level(1), 10);

        var issueId = Guid.NewGuid();

        userProgress.AddIssueProgress(new IssueProgress(
            Guid.NewGuid(),
            Guid.NewGuid(),
            issueId,
            1,
            DateTime.Now,
            TimeSpan.FromMinutes(30),
            Difficulty.Medium)); // Добавляем завершённую задачу

        var lessonId = Guid.NewGuid();
        userProgress.AddLessonProgress(new LessonProgress(
            Guid.NewGuid(),
            lessonId,
            true)); // Завершённый урок

        var issueCondition = new IssueCondition(
            TimeSpan.FromMinutes(60),
            Difficulty.Medium,
            1,
            1); // Условие: выполнить 1 задачу за 60 минут

        var lessonCondition = new LessonCondition(
            TimeSpan.FromMinutes(30),
            Difficulty.Easy,
            1); // Условие: посмотреть 1 урок

        var lessonAchievement = new Achievement(
            Guid.NewGuid(),
            Guid.NewGuid(),
            "",
            "",
            DateOnly.FromDateTime(DateTime.Now),
            lessonCondition,
            0);

        var issueAchievement = new Achievement(
            Guid.NewGuid(),
            Guid.NewGuid(),
            "",
            "",
            DateOnly.FromDateTime(DateTime.Now),
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