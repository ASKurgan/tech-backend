using CSharpFunctionalExtensions;

namespace UserProgressService.Domain.ValueObjects.Ids;

public sealed class IssueProgressId : ComparableValueObject
{
    public IssueProgressId(Guid value)
    {
        Value = value;
    }
    
    public Guid Value { get; init; }
    
    public static IssueProgressId NewId() => new(Guid.NewGuid());
    
    public static IssueProgressId Create(Guid id) => new(id);

    public static implicit operator IssueProgressId(Guid id) => new(id);

    public static implicit operator Guid(IssueProgressId issueProgressId)
    {
        ArgumentNullException.ThrowIfNull(issueProgressId);
        return issueProgressId.Value;
    }
    
    protected override IEnumerable<IComparable> GetComparableEqualityComponents()
    {
        yield return Value;
    }
}