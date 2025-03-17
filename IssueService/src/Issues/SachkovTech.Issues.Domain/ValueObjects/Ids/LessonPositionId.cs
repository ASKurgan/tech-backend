using CSharpFunctionalExtensions;

namespace SachkovTech.Issues.Domain.ValueObjects.Ids;

public class LessonPositionId : ComparableValueObject
{
    public static readonly LessonPositionId Empty = new LessonPositionId(Guid.Empty);

    private LessonPositionId(Guid value)
    {
        Value = value;
    }

    public Guid Value { get; }

    public static LessonPositionId NewLessonPositionId() => new(Guid.NewGuid());

    public static LessonPositionId Create(Guid id) => new(id);

    public static implicit operator LessonPositionId(Guid id) => new(id);

    public static implicit operator Guid(LessonPositionId lessonPositionId)
    {
        ArgumentNullException.ThrowIfNull(lessonPositionId);
        return lessonPositionId.Value;
    }

    protected override IEnumerable<IComparable> GetComparableEqualityComponents()
    {
        yield return Value;
    }
}