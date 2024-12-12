using CSharpFunctionalExtensions;
using SachkovTech.Issues.Domain.Module.ValueObjects;
using SachkovTech.SharedKernel;
using SachkovTech.SharedKernel.ValueObjects;
using SachkovTech.SharedKernel.ValueObjects.Ids;

namespace SachkovTech.Issues.Domain.Module;

public class Module : Entity<ModuleId>, ISoftDeletable
{
    // ef core
    // ReSharper disable once UnusedMember.Local
    public Module(ModuleId moduleId, Title title, Description description)
        : base(moduleId)
    {
        Title = title;
        Description = description;
    }

    private Module(ModuleId id)
        : base(id)
    {
    }

    public Title Title { get; private set; } = default!;

    public Description Description { get; private set; } = default!;

    public IReadOnlyList<IssuePosition> IssuesPosition = [];
    public IReadOnlyList<LessonPosition> LessonsPosition = [];

    public bool IsDeleted { get; private set; }

    public DateTime? DeletionDate { get; private set; }

    public void SoftDelete()
    {
        IsDeleted = true;
        DeletionDate = DateTime.UtcNow;
    }

    public void Restore()
    {
        IsDeleted = false;
        DeletionDate = null;
    }

    public void UpdateIssuesPosition(IEnumerable<IssuePosition> issuesPosition)
    {
        IssuesPosition = issuesPosition.ToList();
    }

    public void UpdateLessonsPosition(IEnumerable<LessonPosition> lessonsPosition)
    {
        LessonsPosition = lessonsPosition.ToList();
    }

    public void UpdateMainInfo(Title title, Description description)
    {
        Title = title;
        Description = description;
    }

    public void AddIssue(IssueId issueId)
    {
        var newIssuePosition = new IssuePosition(
            issueId,
            Position.Create(IssuesPosition.Count + 1).Value);

        var newIssuesPosition = new List<IssuePosition>(IssuesPosition)
        {
            newIssuePosition,
        };

        UpdateIssuesPosition(newIssuesPosition);
    }

    public void AddLesson(LessonId lessonId)
    {
        var newLessonPosition = new LessonPosition(
            lessonId,
            Position.Create(LessonsPosition.Count + 1).Value);

        var newLessonsPosition = new List<LessonPosition>(LessonsPosition)
        {
            newLessonPosition,
        };

        UpdateLessonsPosition(newLessonsPosition);
    }

    public UnitResult<Error> MoveIssue(IssuePosition issuePosition, int newPosition)
    {
        if (issuePosition.Position.Value == newPosition)
            return Result.Success<Error>();

        var rearrangedIssuesPositionResult = ChangePosition(
            IssuesPosition,
            issuePosition.Position.Value,
            newPosition);
        if (rearrangedIssuesPositionResult.IsFailure)
            return rearrangedIssuesPositionResult.Error;

        if (rearrangedIssuesPositionResult.Value.Count != IssuesPosition.Count)
        {
            return Errors.General.Failure();
        }

        UpdateIssuesPosition(rearrangedIssuesPositionResult.Value);
        return Result.Success<Error>();
    }

    public UnitResult<Error> MoveLesson(LessonPosition lessonPosition, Position newPosition)
    {
        if (lessonPosition.Position.Value == newPosition)
            return Result.Success<Error>();

        var rearrangedLessonsPositionResult = ChangePosition(
            LessonsPosition,
            lessonPosition.Position.Value,
            newPosition.Value);
        if (rearrangedLessonsPositionResult.IsFailure)
            return rearrangedLessonsPositionResult.Error;

        if (rearrangedLessonsPositionResult.Value.Count != LessonsPosition.Count)
            return Errors.General.Failure();

        UpdateLessonsPosition(rearrangedLessonsPositionResult.Value);
        return Result.Success<Error>();
    }

    /// <summary>
    /// Generic method to change position for IPositionable in collection 
    /// </summary>
    /// <param name="items"></param>
    /// <param name="positionFrom"></param>
    /// <param name="positionTo"></param>
    /// <returns></returns>
    private Result<List<T>, Error> ChangePosition<T>(
        IReadOnlyList<T> items, int positionFrom, int positionTo) where T : IPositionable
    {
        if (positionFrom == positionTo
            || positionFrom < 1
            || positionTo < 1
            || positionFrom > items.Count
            || positionTo > items.Count)
        {
            return Errors.General.ValueIsInvalid(nameof(Position)); // No adjustment needed or invalid positions
        }

        var increment = positionFrom > positionTo ? 1 : -1;
        var start = int.Min(positionFrom, positionTo);
        var end = int.Max(positionFrom, positionTo);

        List<T> rearrangedItems = [];

        foreach (var currentItem in items)
        {
            var currentPosition = currentItem.Position.Value;

            if (currentPosition >= start && currentPosition <= end)
            {
                if (currentPosition == positionFrom)
                {
                    var newItem = (T)currentItem.Move(Position.Create(positionTo).Value);
                    rearrangedItems.Add(newItem);
                }
                else
                {
                    var newItem = (T)currentItem.Move(
                        Position.Create(currentItem.Position.Value + increment).Value);
                    rearrangedItems.Add(newItem);
                }
            }
            else
            {
                rearrangedItems.Add(currentItem);
            }
        }

        rearrangedItems = rearrangedItems.OrderBy(i => i.Position.Value).ToList();
        return rearrangedItems;
    }

    public UnitResult<Error> DeleteLessonPosition(LessonPosition lessonPosition)
    {
        var copiedList = LessonsPosition.ToList();
        var updateListResult = DeleteItemFromIPositionableCollection(copiedList, lessonPosition);
        if (updateListResult.IsFailure)
            return updateListResult.Error;

        if (updateListResult.Value.Count != (LessonsPosition.Count - 1))
            return Errors.General.Failure();

        UpdateLessonsPosition(updateListResult.Value);
        return Result.Success<Error>();
    }

    public UnitResult<Error> DeleteIssuePosition(Guid issueId)
    {
        var issuePosition = IssuesPosition.FirstOrDefault(x => x.IssueId == issueId);
        if (issuePosition == null)
            return Errors.General.NotFound();

        var copiedList = IssuesPosition.ToList();
        var updateListResult = DeleteItemFromIPositionableCollection(copiedList, issuePosition);
        if (updateListResult.IsFailure)
            return updateListResult.Error;

        if (updateListResult.Value.Count != (IssuesPosition.Count - 1))
            return Errors.General.Failure();

        UpdateIssuesPosition(updateListResult.Value);
        return Result.Success<Error>();
    }

    private Result<List<T>, Error> DeleteItemFromIPositionableCollection<T>(
        List<T> items,
        T itemToDelete) where T : IPositionable
    {
        var result = items.Remove(itemToDelete);
        if (result == false)
            return Errors.General.Failure();

        items = items.OrderBy(i => i.Position.Value).ToList();

        for (int i = 0; i < items.Count; i++)
        {
            if (items[i].Position.Value < itemToDelete.Position.Value)
                continue;
            items[i] = (T)items[i].Move(Position.Create(i + 1).Value);
        }

        return items;
    }
}