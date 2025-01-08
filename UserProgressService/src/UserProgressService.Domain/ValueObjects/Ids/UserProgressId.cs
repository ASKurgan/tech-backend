using CSharpFunctionalExtensions;

namespace UserProgressService.Domain.ValueObjects.Ids;

public sealed class UserProgressId : ComparableValueObject
{
    private UserProgressId(Guid value)
    {
        Value = value;
    }

    public Guid Value { get; init; }
    
    public static UserProgressId NewId() => new(Guid.NewGuid());
    
    public static UserProgressId Create(Guid id) => new(id);

    public static implicit operator UserProgressId(Guid id) => new(id);

    public static implicit operator Guid(UserProgressId userProgressId)
    {
        ArgumentNullException.ThrowIfNull(userProgressId);
        return userProgressId.Value;
    }

    protected override IEnumerable<IComparable> GetComparableEqualityComponents()
    {
        yield return Value;
    }
}