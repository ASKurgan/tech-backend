using CSharpFunctionalExtensions;

namespace SachkovTech.Issues.Domain.ValueObjects.Ids;

public class IssuePositionId : ComparableValueObject
{
    public static readonly IssuePositionId Empty = new IssuePositionId(Guid.Empty);

    private IssuePositionId(Guid value)
    {
        Value = value;
    }

    public Guid Value { get; }

    public static IssuePositionId NewIssuePositionId() => new(Guid.NewGuid());

    public static IssuePositionId Create(Guid id) => new(id);

    public static implicit operator IssuePositionId(Guid id) => new(id);

    public static implicit operator Guid(IssuePositionId issuePositionId)
    {
        ArgumentNullException.ThrowIfNull(issuePositionId);
        return issuePositionId.Value;
    }

    protected override IEnumerable<IComparable> GetComparableEqualityComponents()
    {
        yield return Value;
    }
}