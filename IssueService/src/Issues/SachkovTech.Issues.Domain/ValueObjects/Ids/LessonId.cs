using CSharpFunctionalExtensions;

namespace SachkovTech.Issues.Domain.ValueObjects.Ids;

public class LessonId : ComparableValueObject
{
    private LessonId(Guid value)
    {
        Value = value;
    }

    public Guid Value { get; }

    public static LessonId NewLessonId() => new LessonId(Guid.NewGuid());

    public static LessonId Create(Guid id) => new(id);

    public static implicit operator LessonId(Guid id) => new(id);

    public static implicit operator Guid(LessonId lessonId)
    {
        ArgumentNullException.ThrowIfNull(lessonId);
        return lessonId.Value;
    }

    protected override IEnumerable<IComparable> GetComparableEqualityComponents()
    {
        yield return Value;
    }
}
