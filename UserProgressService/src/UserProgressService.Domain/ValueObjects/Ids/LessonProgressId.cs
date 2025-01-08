using CSharpFunctionalExtensions;

namespace UserProgressService.Domain.ValueObjects.Ids;

public sealed class LessonProgressId : ComparableValueObject
{
    private LessonProgressId(Guid value)
    {
        Value = value;
    }

    public Guid Value { get; init; }
    
    public static LessonProgressId NewId() => new(Guid.NewGuid());
    
    public static LessonProgressId Create(Guid id) => new(id);

    public static implicit operator LessonProgressId(Guid id) => new(id);

    public static implicit operator Guid(LessonProgressId lessonProgressId)
    {
        ArgumentNullException.ThrowIfNull(lessonProgressId);
        return lessonProgressId.Value;
    }
    
    protected override IEnumerable<IComparable> GetComparableEqualityComponents()
    {
        yield return Value;
    }
}