using System.Text.Json.Serialization;
using CSharpFunctionalExtensions;
using SachkovTech.Issues.Domain.ValueObjects.Ids;

namespace SachkovTech.Issues.Domain.Module.ValueObjects;

public class LessonPosition : ComparableValueObject, IPositionable
{
    [JsonConstructor]
    public LessonPosition(LessonId lessonId, Position position)
    {
        LessonId = lessonId;
        Position = position;
    }

    public LessonId LessonId { get; }

    public Position Position { get; }

    public IPositionable Move(Position newPosition) => new LessonPosition(LessonId, newPosition);

    protected override IEnumerable<IComparable> GetComparableEqualityComponents()
    {
        yield return LessonId;
        yield return Position;
    }
}