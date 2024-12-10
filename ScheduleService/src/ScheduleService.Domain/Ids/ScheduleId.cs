using CSharpFunctionalExtensions;

namespace ScheduleService.Domain.Ids;

public class ScheduleId : ComparableValueObject
{
    private ScheduleId() { }

    private ScheduleId(Guid value)
    {
        Value = value;
    }


    public Guid Value { get; }

    public static ScheduleId New() => new(Guid.NewGuid());
    public static ScheduleId Empty() => new(Guid.Empty);
    public static ScheduleId Create(Guid id) => new(id);

    protected override IEnumerable<IComparable> GetComparableEqualityComponents()
    {
        yield return Value;
    }
}
