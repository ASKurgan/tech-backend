using CSharpFunctionalExtensions;

namespace SachkovTech.Issues.Domain.ValueObjects.Ids;

public class CommentId : ComparableValueObject
{
    private CommentId(Guid value)
    {
        Value = value;
    }

    public Guid Value { get; init; }

    public static CommentId NewCommentId() => new(Guid.NewGuid());

    public static CommentId Empty() => new(Guid.Empty);

    public static CommentId Create(Guid id) => new(id);

    protected override IEnumerable<IComparable> GetComparableEqualityComponents()
    {
        yield return Value;
    }
}