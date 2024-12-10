using CSharpFunctionalExtensions;

namespace ScheduleService.Domain.Ids;

public class EventId : ComparableValueObject
{
    private EventId() { }

    private EventId(Guid value)
    {
        Value = value;
    }


    public Guid Value { get; }

    public static EventId New() => new(Guid.NewGuid());
    public static EventId Empty() => new(Guid.Empty);
    public static EventId Create(Guid id) => new(id);

    protected override IEnumerable<IComparable> GetComparableEqualityComponents()
    {
        yield return Value;
    }
}
