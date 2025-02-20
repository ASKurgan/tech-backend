using FluentAssertions;
using SachkovTech.Issues.Domain.Module;
using SachkovTech.Issues.Domain.Module.ValueObjects;
using SachkovTech.Issues.Domain.ValueObjects;
using SachkovTech.Issues.Domain.ValueObjects.Ids;

namespace SachkovTech.Issues.UnitTests.Domain;

public class ModuleTests
{
    [Fact]
    public void MoveLesson_forward()
    {
        // Arrange
        var module = CreateAndFillModule(10);

        var lessonToMove = module.LessonsPosition.First(x => x.Position.Value == 3);
        var lessonToCheck = module.LessonsPosition.First(x => x.Position.Value == 6);

        // Act
        var result = module.MoveLesson(lessonToMove, Position.Create(6).Value);

        // Assert
        result.IsSuccess.Should().BeTrue();
        module.LessonsPosition.Should().HaveCount(10);
        module.LessonsPosition.First(x => x.Position.Value == 6).LessonId.Should().Be(lessonToMove.LessonId);
        module.LessonsPosition.First(x => x.Position.Value == 5).LessonId.Should().Be(lessonToCheck.LessonId);
    }

    [Fact]
    public void MoveLesson_backward()
    {
        // Arrange
        var module = CreateAndFillModule(10);

        var lessonToMove = module.LessonsPosition.First(x => x.Position.Value == 7);
        var lessonToCheck = module.LessonsPosition.First(x => x.Position.Value == 4);

        // Act
        var result = module.MoveLesson(lessonToMove, Position.Create(4).Value);

        // Assert
        result.IsSuccess.Should().BeTrue();
        module.LessonsPosition.Should().HaveCount(10);
        module.LessonsPosition.First(x => x.Position.Value == 4).LessonId.Should().Be(lessonToMove.LessonId);
        module.LessonsPosition.First(x => x.Position.Value == 5).LessonId.Should().Be(lessonToCheck.LessonId);
    }

    [Fact]
    public void MoveLesson_ToTheSamePosition_ShouldBeWithoutChanges()
    {
        // Arrange
        var module = CreateAndFillModule(10);

        var lessonToMove = module.LessonsPosition.First(x => x.Position.Value == 7);
        var lessonToCheck = module.LessonsPosition.First(x => x.Position.Value == 6);

        // Act
        var result = module.MoveLesson(lessonToMove, Position.Create(7).Value);

        // Assert
        result.IsSuccess.Should().BeTrue();
        module.LessonsPosition.Should().HaveCount(10);
        module.LessonsPosition.First(x => x.Position.Value == 7).LessonId.Should().Be(lessonToMove.LessonId);
        module.LessonsPosition.First(x => x.Position.Value == 6).LessonId.Should().Be(lessonToCheck.LessonId);
    }

    [Fact]
    public void MoveLesson_ToOutOfRangePosition_ShouldBeFailure()
    {
        // Arrange
        var module = CreateAndFillModule(10);

        var lessonToMove = module.LessonsPosition.First(x => x.Position.Value == 4);

        // Act
        var result = module.MoveLesson(lessonToMove, Position.Create(11).Value);

        // Assert
        result.IsSuccess.Should().BeFalse();
        module.LessonsPosition.Should().HaveCount(10);
    }

    [Fact]
    public void MoveIssue_forward()
    {
        // Arrange
        var module = CreateAndFillModule(10);

        var issueToMove = module.IssuesPosition.First(x => x.Position.Value == 3);
        var issueToCheck = module.IssuesPosition.First(x => x.Position.Value == 6);

        // Act
        var result = module.MoveIssue(issueToMove, Position.Create(6).Value);

        // Assert
        result.IsSuccess.Should().BeTrue();
        module.IssuesPosition.Should().HaveCount(10);
        module.IssuesPosition.First(x => x.Position.Value == 6).IssueId.Should().Be(issueToMove.IssueId);
        module.IssuesPosition.First(x => x.Position.Value == 5).IssueId.Should().Be(issueToCheck.IssueId);
    }

    [Fact]
    public void MoveIssue_backward()
    {
        // Arrange
        var module = CreateAndFillModule(10);

        var issueToMove = module.IssuesPosition.First(x => x.Position.Value == 7);
        var issueToCheck = module.IssuesPosition.First(x => x.Position.Value == 4);

        // Act
        var result = module.MoveIssue(issueToMove, Position.Create(4).Value);

        // Assert
        result.IsSuccess.Should().BeTrue();
        module.IssuesPosition.Should().HaveCount(10);
        module.IssuesPosition.First(x => x.Position.Value == 4).IssueId.Should().Be(issueToMove.IssueId);
        module.IssuesPosition.First(x => x.Position.Value == 5).IssueId.Should().Be(issueToCheck.IssueId);
    }

    [Fact]
    public void MoveIssue_ToTheSamePosition_ShouldBeWithoutChanges()
    {
        // Arrange
        var module = CreateAndFillModule(10);

        var issueToMove = module.IssuesPosition.First(x => x.Position.Value == 7);
        var issueToCheck = module.IssuesPosition.First(x => x.Position.Value == 6);

        // Act
        var result = module.MoveIssue(issueToMove, Position.Create(7).Value);

        // Assert
        result.IsSuccess.Should().BeTrue();
        module.IssuesPosition.Should().HaveCount(10);
        module.IssuesPosition.First(x => x.Position.Value == 7).IssueId.Should().Be(issueToMove.IssueId);
        module.IssuesPosition.First(x => x.Position.Value == 6).IssueId.Should().Be(issueToCheck.IssueId);
    }

    [Fact]
    public void MoveIssue_ToOutOfRangePosition_ShouldBeFailure()
    {
        // Arrange
        var module = CreateAndFillModule(10);

        var issueToMove = module.IssuesPosition.First(x => x.Position.Value == 4);

        // Act
        var result = module.MoveIssue(issueToMove, Position.Create(11).Value);

        // Assert
        result.IsSuccess.Should().BeFalse();
        module.IssuesPosition.Should().HaveCount(10);
    }

    [Fact]
    public void MoveIssue_to_forward()
    {
        // Arrange
        var module = new Module(
            ModuleId.NewModuleId(),
            Title.Create("test title").Value,
            Description.Create("test description").Value);

        var issueGuid1 = IssueId.Create(Guid.NewGuid());
        var issueGuid2 = IssueId.Create(Guid.NewGuid());
        var issueGuid3 = IssueId.Create(Guid.NewGuid());

        var position1 = Position.Create(1).Value;
        var position2 = Position.Create(2).Value;
        var position3 = Position.Create(3).Value;

        var issuesPosition = new List<IssuePosition>
        {
            new IssuePosition(issueGuid1, position1),
            new IssuePosition(issueGuid2, position2),
            new IssuePosition(issueGuid3, position3),
        };

        module.IssuesPosition = issuesPosition;

        var issuesPositions = module.IssuesPosition.OrderBy(i => i.Position.Value).ToList();

        // Act
        var result = module.MoveIssue(issuesPositions[1], Position.Create(3).Value);

        // Assert
        var finalIssues = module.IssuesPosition.OrderBy(i => i.Position.Value).ToList();

        result.IsSuccess.Should().BeTrue();
        finalIssues[0].IssueId.Value.Should().Be(issueGuid1.Value);
        finalIssues[1].IssueId.Value.Should().Be(issueGuid3.Value);
        finalIssues[2].IssueId.Value.Should().Be(issueGuid2.Value);
    }

    [Fact]
    public void MoveIssue_to_last_position()
    {
        // Arrange
        var module = new Module(
            ModuleId.NewModuleId(),
            Title.Create("test title").Value,
            Description.Create("test description").Value);

        var issueGuid1 = IssueId.Create(Guid.NewGuid());
        var issueGuid2 = IssueId.Create(Guid.NewGuid());
        var issueGuid3 = IssueId.Create(Guid.NewGuid());

        var position1 = Position.Create(1).Value;
        var position2 = Position.Create(2).Value;
        var position3 = Position.Create(3).Value;

        var issuesPosition = new List<IssuePosition>
        {
            new IssuePosition(issueGuid1, position1),
            new IssuePosition(issueGuid2, position2),
            new IssuePosition(issueGuid3, position3),
        };

        module.IssuesPosition = issuesPosition;

        var issuesPositions = module.IssuesPosition.OrderBy(i => i.Position.Value).ToList();

        // Act
        var result = module.MoveIssue(issuesPositions[0], Position.Create(3).Value);

        // Assert
        var finalIssues = module.IssuesPosition.OrderBy(i => i.Position.Value).ToList();

        result.IsSuccess.Should().BeTrue();
        finalIssues[0].IssueId.Value.Should().Be(issueGuid2.Value);
        finalIssues[1].IssueId.Value.Should().Be(issueGuid3.Value);
        finalIssues[2].IssueId.Value.Should().Be(issueGuid1.Value);
    }

    [Fact]
    public void DeleteIssue_ShouldBeNotFoundInList()
    {
        // Arrange
        var module = CreateAndFillModule(10);

        var issueToDelete = module.IssuesPosition.First(x => x.Position.Value == 4);

        // Act
        var result = module.DeleteIssuePosition(issueToDelete.IssueId);

        // Assert
        result.IsSuccess.Should().BeTrue();
        module.IssuesPosition.Should().HaveCount(9);
        module.IssuesPosition.FirstOrDefault(x => x.IssueId == issueToDelete.IssueId)
            .Should().Be(null);

        bool isSortedAndIncrementing = module.IssuesPosition
            .Select((item, index) => item.Position.Value - index)
            .Distinct()
            .Count() == 1;
        isSortedAndIncrementing.Should().BeTrue();
    }

    [Fact]
    public void DeleteLesson_ShouldBeNotFoundInList()
    {
        // Arrange
        var module = CreateAndFillModule(10);

        var lessonToDelete = module.LessonsPosition.First(x => x.Position.Value == 4);

        // Act
        var result = module.DeleteLessonPosition(lessonToDelete.LessonId);

        // Assert
        result.IsSuccess.Should().BeTrue();
        module.LessonsPosition.Should().HaveCount(9);
        module.LessonsPosition.FirstOrDefault(x => x.LessonId == lessonToDelete.LessonId)
            .Should().Be(null);

        bool isSortedAndIncrementing = module.LessonsPosition
            .Select((item, index) => item.Position.Value - index)
            .Distinct()
            .Count() == 1;
        isSortedAndIncrementing.Should().BeTrue();
    }

    [Fact]
    public void Soft_delete_module()
    {
        // Arrange
        var module = CreateAndFillModule(3);

        // Act
        module.SoftDelete();

        // Assert
        module.IsDeleted.Should().BeTrue();
    }

    [Fact]
    public void Restore_module()
    {
        // Arrange
        var module = CreateAndFillModule(3);
        module.SoftDelete();

        // Act
        module.Restore();

        // Assert
        module.IsDeleted.Should().BeFalse();
    }

    private Module CreateAndFillModule(int collectionsItemsCount)
    {
        var module = new Module(
            ModuleId.NewModuleId(),
            Title.Create("test title").Value,
            Description.Create("test description").Value);

        var issuesPosition = new List<IssuePosition>();
        var lessonsPosition = new List<LessonPosition>();

        for (int i = 1; i < collectionsItemsCount + 1; i++)
        {
            issuesPosition.Add(new IssuePosition(Guid.NewGuid(), Position.Create(i).Value));
            lessonsPosition.Add(new LessonPosition(Guid.NewGuid(), Position.Create(i).Value));
        }

        module.IssuesPosition = issuesPosition;
        module.LessonsPosition = lessonsPosition;

        return module;
    }
}