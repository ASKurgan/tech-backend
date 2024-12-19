using CSharpFunctionalExtensions;

namespace UserProgressService.Domain.ValueObjects.Ids;

public sealed class AchievementId : ComparableValueObject
{
    public AchievementId(Guid value)
    {
        Value = value;
    }

    public Guid Value { get; init; }

    public static AchievementId NewId() => new(Guid.NewGuid());

    public static AchievementId Create(Guid id) => new(id);

    public static implicit operator AchievementId(Guid id) => new(id);

    public static implicit operator Guid(AchievementId achievementId)
    {
        ArgumentNullException.ThrowIfNull(achievementId);
        return achievementId.Value;
    }

    protected override IEnumerable<IComparable> GetComparableEqualityComponents()
    {
        yield return Value;
    }
}