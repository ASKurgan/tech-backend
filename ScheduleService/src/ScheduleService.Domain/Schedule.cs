using CSharpFunctionalExtensions;
using ScheduleService.Domain.Entities;
using ScheduleService.Domain.Ids;
using ScheduleService.Domain.Shared;
using ScheduleService.Domain.ValueObjects;
using SharedKernel;

namespace ScheduleService.Domain;

public abstract class Schedule : DomainEntity<ScheduleId>
{
    protected Schedule(ScheduleId id) : base(id)
    {
    }

    protected Schedule(
        ScheduleId id,
        Title title,
        Description description,
        DateTime startDate,
        DateTime endDate,
        TimeSpan eventDuration,
        bool isAutomaticRenewal
    ) : base(id)
    {
        Title = title;
        Description = description;
        StartDate = startDate;
        EndDate = endDate;
        EventDuration = eventDuration;
        IsAutomaticRenewal = isAutomaticRenewal;
        _plannedEvents = [];
    }

    private List<EventInstance> _plannedEvents = [];

    public Title Title { get; private set; }

    public Description Description { get; private set; }

    public bool IsAutomaticRenewal { get; private set; }

    public DateTime StartDate { get; protected set; }

    public DateTime EndDate { get; protected set; }

    protected TimeSpan EventDuration { get; set; }
    public IReadOnlyList<EventInstance> PlannedEvents => _plannedEvents;

    public UnitResult<Error> AddEvent(EventInstance eventInstance)
    {
        if (_plannedEvents.Any(e => e.Id == eventInstance.Id))
            return Errors.General.AlreadyExist();

        if (eventInstance.Start < StartDate && eventInstance.Start > EndDate)
            return ErrorsSchedule.Schedule.InvalidPlannedEvent();

        _plannedEvents.Add(eventInstance);

        return UnitResult.Success<Error>();
    }

    public UnitResult<Error> RescheduleEvent(EventInstance eventInstance, DateTime newDate)
    {
        var eventInstanceToUpdate = _plannedEvents.FirstOrDefault(e => e.Id == eventInstance.Id);
        if (eventInstanceToUpdate is null)
            return Errors.General.NotFound(eventInstance.Id.Value, "event instance");

        eventInstanceToUpdate.Reschedule(newDate);

        return UnitResult.Success<Error>();
    }

    public UnitResult<Error> RemoveEvent(EventInstance eventInstance)
    {
        if (_plannedEvents.Any(e => e.Id == eventInstance.Id) == false)
            return Errors.General.NotFound(eventInstance.Id.Value, "event instance");

        _plannedEvents.Remove(eventInstance);

        return UnitResult.Success<Error>();
    }

    public UnitResult<Error> UnableAutomaticRenewal()
    {
        if (IsAutomaticRenewal)
            return ErrorsSchedule.Schedule.RenewalIsEnabled();

        IsAutomaticRenewal = true;
        return UnitResult.Success<Error>();
    }

    public UnitResult<Error> DisableAutomaticRenewal()
    {
        if (!IsAutomaticRenewal)
            return ErrorsSchedule.Schedule.RenewalIsDisabled();

        IsAutomaticRenewal = false;
        return UnitResult.Success<Error>();
    }

    public void ChangeDurationForAllEvents(TimeSpan newTime)
    {
        foreach (var e in _plannedEvents)
            e.SetDuration(newTime);
    }

    public void UpdateSchedule(Title title, Description description) => (Title, Description) = (title, description);
}