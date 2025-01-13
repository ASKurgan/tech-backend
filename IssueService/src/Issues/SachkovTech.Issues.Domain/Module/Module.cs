using CSharpFunctionalExtensions;
using SachkovTech.Issues.Domain.Module.ValueObjects;
using SachkovTech.Issues.Domain.ValueObjects;
using SachkovTech.Issues.Domain.ValueObjects.Ids;
using SharedKernel;

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

        var newIssuesPosition = new List<IssuePosition>(IssuesPosition) { newIssuePosition, };

        IssuesPosition = newIssuesPosition;
    }

    public void AddLesson(LessonId lessonId)
    {
        var newLessonPosition = new LessonPosition(
            lessonId,
            Position.Create(LessonsPosition.Count + 1).Value);

        var newLessonsPosition = new List<LessonPosition>(LessonsPosition) { newLessonPosition, };

        LessonsPosition = newLessonsPosition;
    }

    public UnitResult<Error> MoveIssue(IssuePosition issuePosition, Position newPosition)
    {
        var updatedListResult = ChangePosition(IssuesPosition, issuePosition.Position, newPosition);

        if (updatedListResult.IsFailure)
            return updatedListResult.Error;

        IssuesPosition = updatedListResult.Value;
        return Result.Success<Error>();
    }

    public UnitResult<Error> MoveLesson(LessonPosition lessonPosition, Position newPosition)
    {
        var updatedListResult = ChangePosition(LessonsPosition, lessonPosition.Position, newPosition);

        if (updatedListResult.IsFailure)
            return updatedListResult.Error;

        LessonsPosition = updatedListResult.Value;
        return Result.Success<Error>();
    }

    /// <summary>
    /// Удаляет позицию задачи по её IssueId. После удаления пересчитывает позиции оставшихся задач
    /// и обновляет список позиций.
    /// </summary>
    /// <param name="issueId">Идентификатор задачи, позиция которой будет удалена.</param>
    /// <returns>Успешный результат операции или ошибка, если задача не найдена.</returns>
    public UnitResult<Error> DeleteIssuePosition(IssueId issueId)
    {
        var issuePosition = IssuesPosition.FirstOrDefault(x => x.IssueId == issueId);
        if (issuePosition == null)
            return Errors.General.NotFound();

        var updatedListResult = RemovePositionAndReorder(IssuesPosition, issuePosition);
        if (updatedListResult.IsFailure)
            return updatedListResult.Error;

        IssuesPosition = updatedListResult.Value.Cast<IssuePosition>().ToList();
        return Result.Success<Error>();
    }

    /// <summary>
    /// Удаляет позицию урока по его LessonId. После удаления пересчитывает позиции оставшихся уроков
    /// и обновляет список позиций.
    /// </summary>
    /// <param name="lessonId">Идентификатор урока, позиция которого будет удалена.</param>
    /// <returns>Успешный результат операции или ошибка, если урок не найден.</returns>
    public UnitResult<Error> DeleteLessonPosition(LessonId lessonId)
    {
        var lessonPosition = LessonsPosition.FirstOrDefault(x => x.LessonId == lessonId);
        if (lessonPosition == null)
            return Errors.General.NotFound();

        var updateListResult = RemovePositionAndReorder(LessonsPosition, lessonPosition);
        if (updateListResult.IsFailure)
            return updateListResult.Error;

        LessonsPosition = updateListResult.Value.Cast<LessonPosition>().ToList();
        return Result.Success<Error>();
    }

    /// <summary>
    /// Перемещает элемент в коллекции на новую позицию, при этом пересчитывает позиции других элементов.
    /// Возвращает новую отсортированную коллекцию.
    /// </summary>
    /// <param name="items">Коллекция элементов, которые нужно отсортировать и переместить.</param>
    /// <param name="positionFrom">Позиция элемента, который нужно переместить.</param>
    /// <param name="positionTo">Новая позиция для перемещения элемента.</param>
    /// <returns>Отсортированную коллекцию с измененными позициями или ошибку если что-то пошло не так.</returns>
    private Result<List<T>, Error> ChangePosition<T>(
        IReadOnlyList<T> items, Position positionFrom, Position positionTo)
        where T : IPositionable
    {
        if (positionTo == positionFrom)
            return items.ToList();

        if (positionFrom < 1
            || positionTo < 1
            || positionFrom > items.Count
            || positionTo > items.Count)
        {
            return Errors.General.ValueIsInvalid(nameof(Position));
        }

        var reorderedList = items
            .Select((item, index) => new { Item = item, Index = index + 1 })
            .OrderBy(x =>
            {
                if (x.Index == positionFrom) return positionTo;
                if (x.Index >= Math.Min(positionFrom, positionTo) && x.Index <= Math.Max(positionFrom, positionTo))
                {
                    return x.Index < positionFrom ? x.Index + 1 : x.Index - 1;
                }

                return x.Index;
            })
            .Select((x, newIndex) => (T)x.Item.Move(Position.Create(newIndex + 1).Value))
            .ToList();

        return reorderedList;
    }

    /// <summary>
    /// Удаляет указанный элемент из коллекции и пересчитывает позиции оставшихся элементов.
    /// Возвращает обновленный список.
    /// </summary>
    /// <param name="items">Коллекция элементов, из которой будет удален элемент.</param>
    /// <param name="itemToRemove">Элемент, который будет удален.</param>
    /// <returns>Обновленный список элементов с пересчитанными позициями или ошибку если что-то пошло не так.</returns>
    private Result<List<IPositionable>, Error> RemovePositionAndReorder(
        IReadOnlyList<IPositionable> items, IPositionable itemToRemove)
    {
        if (!items.Contains(itemToRemove))
            return Errors.General.NotFound();

        var reorderedList = items
            .Where(x => x != itemToRemove)
            .Select((item, index) => item.Move(Position.Create(index + 1).Value))
            .ToList();

        return reorderedList;
    }
}