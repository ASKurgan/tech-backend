using CSharpFunctionalExtensions;
using ScheduleService.Domain.Entities;
using ScheduleService.Domain.Enums;
using ScheduleService.Domain.Ids;
using ScheduleService.Domain.Shared;
using ScheduleService.Domain.ValueObjects;
using SharedKernel;

namespace ScheduleService.Domain.TypeSchedules;

public sealed class DailySchedule : Schedule
{
    private DailySchedule(ScheduleId id) : base(id) { }

    private DailySchedule(
        ScheduleId id,
        DateTime startDate,
        DateTime endDate,
        TimeSpan eventDuration,
        List<TimeSpan> executionTimes,
        Title title,
        Description description,
        bool isAutomaticRenewal) : base(id, title, description, startDate, endDate, eventDuration, isAutomaticRenewal)
    {
        _executionTimes = executionTimes;
    }
    private readonly List<TimeSpan> _executionTimes;

    public IReadOnlyList<TimeSpan> ExecutionTimes => _executionTimes;

    /// <summary>
    /// Создает ежедневное расписание с указанными параметрами и генерирует события для каждого дня в диапазоне дат.
    /// </summary>
    /// <param name="id">Уникальный идентификатор расписания.</param>
    /// <param name="startDate">Дата начала расписания.</param>
    /// <param name="endDate">Дата окончания расписания.</param>
    /// <param name="duration">Время начала для всех событий.</param>
    /// <param name="title">Заголовок расписания.</param>
    /// <param name="description">Описание расписания.</param>
    /// <param name="executionTimeList">Список времени выполнения для каждого дня.</param>
    /// <param name="isAutomaticRenewal">Флаг автоматического продления расписания.</param>
    /// <returns>Результат создания расписания или ошибка.</returns>
    public static Result<DailySchedule, Error> Create(
        ScheduleId id,
        DateTime startDate,
        DateTime endDate,
        List<TimeSpan> executionTimeList,
        TimeSpan duration,
        Title title,
        Description description,
        bool isAutomaticRenewal)
    {
        if (!executionTimeList.Any())
            return Errors.General.ValueIsInvalid("executionTimeList");

        if (startDate > endDate)
            return Errors.General.ValueIsInvalid("endDate");

        var schedule = new DailySchedule(id,
            startDate,
            endDate,
            duration,
            executionTimeList,
            title,
            description,
            isAutomaticRenewal);

        var isEventsAdded = schedule.AddEvents();
        if (isEventsAdded.IsFailure)
            return isEventsAdded.Error;

        return schedule;
    }

    /// <summary>
    /// Продлевает расписание, добавляя новые события без ограничения по количеству.
    /// </summary>
    /// <returns>Результат операции или ошибка.</returns>
    public UnitResult<Error> ExtendEvents()
    {
        if (!IsAutomaticRenewal)
            return ErrorsSchedule.Schedule.RenewalIsDisabled();
        
        var events = ExtendSchedule(EndDate);
        foreach (var newEvent in events)
        {
            var isEventAdded = AddEvent(newEvent);
            if (isEventAdded.IsFailure)
                return isEventAdded.Error;
        }
        
        var lastEvent = PlannedEvents[^1];
        EndDate = lastEvent.Start.Add(lastEvent.Duration);
        
        return UnitResult.Success<Error>();
    }

    /// <summary>
    /// Генерирует события на основе времени выполнения и диапазона дат расписания.
    /// </summary>
    /// <param name="startDate">Дата начала расписания.</param>
    /// <returns>Список событий.</returns>
    private IEnumerable<EventInstance> ExtendSchedule(DateTime startDate)
    {
        return ExecutionTimes.Select(time => new EventInstance(EventId.New(),
            startDate.AddDays(1).Date.Add(time),
            Id,
            EventDuration,
            EventStatus.Scheduled));
    }

    /// <summary>
    /// Добавляет события на основе времени выполнения и диапазона дат расписания.
    /// </summary>
    /// <returns>Возвращает результат операции или ошибку.</returns>
    private UnitResult<Error> AddEvents()
    {
        var countDays = (EndDate - StartDate).Days;

        var result = Enumerable
            .Range(0, countDays)
            .SelectMany(i =>
                ExecutionTimes.Select(time => new EventInstance(
                    EventId.New(),
                    StartDate.AddDays(i).Date.Add(time),
                    Id,
                    EventDuration,
                    EventStatus.Scheduled)))
            .Select(AddEvent)
            .FirstOrDefault(isEventAdded => isEventAdded.IsFailure);

        return result.IsFailure ? result.Error : UnitResult.Success<Error>();
    }
}
