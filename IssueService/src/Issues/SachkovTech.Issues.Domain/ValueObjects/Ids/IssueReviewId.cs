using CSharpFunctionalExtensions;

namespace SachkovTech.Issues.Domain.ValueObjects.Ids;

public class IssueReviewId : ComparableValueObject
{
    private IssueReviewId(Guid value)
    {
        Value = value;
    }

    public Guid Value { get; init; }

    public static IssueReviewId NewIssueReviewId() => new(Guid.NewGuid());

    public static IssueReviewId Empty() => new(Guid.Empty);

    public static IssueReviewId Create(Guid id) => new(id);

    public static implicit operator Guid(IssueReviewId issueReviewId) => issueReviewId.Value;

    protected override IEnumerable<IComparable> GetComparableEqualityComponents()
    {
        yield return Value;
    }
}