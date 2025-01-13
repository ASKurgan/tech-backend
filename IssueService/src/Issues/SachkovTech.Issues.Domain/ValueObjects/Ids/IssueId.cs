using CSharpFunctionalExtensions;

namespace SachkovTech.Issues.Domain.ValueObjects.Ids;

public class IssueId : ComparableValueObject
{
    private IssueId(Guid value)
    {
        Value = value;
    }

    public Guid Value { get; init; }

    public static IssueId NewIssueId() => new(Guid.NewGuid());

    public static IssueId Empty() => new(Guid.Empty);

    public static IssueId Create(Guid id) => new(id);

    public static implicit operator IssueId(Guid id) => new(id);

    public static implicit operator Guid(IssueId issueId)
    {
        ArgumentNullException.ThrowIfNull(issueId);
        return issueId.Value;
    }

    protected override IEnumerable<IComparable> GetComparableEqualityComponents()
    {
        yield return Value;
    }
}