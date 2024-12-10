using CSharpFunctionalExtensions;
using ScheduleService.Domain.Enums;
using ScheduleService.Domain.Ids;
using SharedKernel;

namespace ScheduleService.Domain.Entities;

public class EventInstance : Entity<EventId>
{
    private EventInstance(EventId id) : base(id) { }

    public EventInstance(EventId id, DateTime start, ScheduleId scheduleId, TimeSpan duration, EventStatus status) : base(id)
    {
        Start = start;
        Duration = duration;
        ScheduleId = scheduleId;
        Status = status;
    }
    
    public DateTime Start { get; private set; }
    
    public TimeSpan Duration { get; private set; } 

    public ScheduleId ScheduleId { get; private set; }

    public EventStatus Status { get; private set; }

    public void Reschedule(DateTime newDate)
    {
        Start = newDate;
        Status = EventStatus.Rescheduled;
    }

    public void Cancel() => Status = EventStatus.Cancelled;
    public void Finish() => Status = EventStatus.Finished;
    public void SetDuration(TimeSpan duration) => Duration = duration;
}
