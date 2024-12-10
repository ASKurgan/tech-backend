using CSharpFunctionalExtensions;
using ScheduleService.Domain.Ids;
using SharedKernel;

namespace ScheduleService.Domain.ValueObjects;

public class EventTime : ComparableValueObject
{
    public DayOfWeek Day { get; }
    public TimeSpan Time { get; }

    private EventTime() { }
    private EventTime(DayOfWeek day, TimeSpan time)
    {
        Day = day;
        Time = time;
    }

    public static Result<EventTime, Error> Create(DayOfWeek day, TimeSpan time)
    {
        if (time.Hours > 24) 
        {
            // TODO: add error
        }
        return new EventTime(day, time);
    }

    protected override IEnumerable<IComparable> GetComparableEqualityComponents()
    {
        yield return Day;
        yield return Time;
    }
}
