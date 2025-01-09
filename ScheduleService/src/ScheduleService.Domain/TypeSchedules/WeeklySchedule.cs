using CSharpFunctionalExtensions;
using ScheduleService.Domain.Entities;
using ScheduleService.Domain.Enums;
using ScheduleService.Domain.Ids;
using ScheduleService.Domain.Shared;
using ScheduleService.Domain.ValueObjects;
using SharedKernel;

namespace ScheduleService.Domain.TypeSchedules;

public class WeeklySchedule : Schedule
{
    public const int DAYS_IN_WEEK = 7;

    private WeeklySchedule(ScheduleId id) : base(id)
    {
    }

    private WeeklySchedule(
        ScheduleId id,
        DateTime startDate,
        DateTime endDate,
        TimeSpan eventDuration,
        List<EventTime> executionDays,
        Title title,
        Description description,
        int repeatInterval,
        bool isAutomaticRenewal) : base(id, title, description, startDate, endDate, eventDuration, isAutomaticRenewal)
    {
        RepeatInterval = repeatInterval;
        _executionDays = executionDays;
    }

    private List<EventTime> _executionDays;
    public int RepeatInterval { get; private set; }

    public IReadOnlyCollection<EventTime> ExecutionDays => _executionDays;


    /// <summary>
    ///    Создание еженедельного расписания.
    /// </summary>
    /// <param name="id">Id расписания.</param>
    /// <param name="startDate">Начало расписания.</param>
    /// <param name="endDate">Конец расписания</param>
    /// <param name="executionDays">Запланированные пользователем даты на занятия каждой недели.</param>
    /// <param name="duration">Время начала для всех событий.</param>
    /// <param name="title">Заголовок расписания.</param>
    /// <param name="description">Описание расписания.</param>
    /// <param name="repeatInterval">Количество повторений расписания.</param>
    /// <param name="isAutomaticRenewal">Автоматическое продление.</param>
    /// <returns>Возвращает расписание или ошибку при создании.</returns>
    public static Result<WeeklySchedule, Error> Create(
        ScheduleId id,
        DateTime startDate,
        DateTime endDate,
        TimeSpan duration,
        List<EventTime> executionDays,
        Title title,
        Description description,
        int repeatInterval,
        bool isAutomaticRenewal)
    {
        if (endDate < startDate)
            return Errors.General.ValueIsInvalid("endDate");

        if (repeatInterval < 0)
            return Errors.General.ValueIsInvalid("repeatInterval");

        if (!executionDays.Any())
            return Errors.General.ValueIsInvalid("executionTimeList");

        var schedule = new WeeklySchedule(id,
            startDate,
            endDate,
            duration,
            executionDays,
            title,
            description,
            repeatInterval,
            isAutomaticRenewal);

        var result = schedule.AddEvents();
        if (result.IsFailure)
            return result.Error;

        return schedule;
    }

    /// <summary>
    /// Продлевает расписание, добавляя новые события.
    /// </summary>
    /// <returns>Результат операции или ошибка.</returns>
    public UnitResult<Error> ExtendEvents()
    {
        if (!IsAutomaticRenewal)
            return ErrorsSchedule.Schedule.RenewalIsDisabled();

        var events = ExtendSchedule(EndDate.AddDays(1));
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
    /// Логика продления расписания.
    /// </summary>
    /// <param name="startDay">Дата начала продления.</param>
    private IEnumerable<EventInstance> ExtendSchedule(DateTime startDay)
    {
        return ExecutionDays.Select(executionDay =>
        {
            var targetDate = CalculateDateTime(startDay.AddDays(1 * DAYS_IN_WEEK + 1),
                executionDay.Day,
                executionDay.Time);

            return new EventInstance(EventId.New(), targetDate, Id, EventDuration, EventStatus.Scheduled);
        });
    }

    /// <summary>
    /// Генерирует запланированные события для нескольких недель, начиная с заданной даты. 
    /// </summary>
    private UnitResult<Error> AddEvents()
    {
        var result = Enumerable
            .Range(0, RepeatInterval)
            .SelectMany(i => GeneratePlannedEvents(StartDate.AddDays(i * DAYS_IN_WEEK)))
            .Select(AddEvent)
            .FirstOrDefault(isEventAdded => isEventAdded.IsFailure);

        return result.IsFailure ? result.Error : UnitResult.Success<Error>();
    }

    /// <summary>
    /// Добавляет события в расписание для каждой из запланированных дней в неделе. 
    /// </summary>
    /// <param name="startDateOfWeek">Дата начала недели, с которой начинается расчет.</param>
    private IEnumerable<EventInstance> GeneratePlannedEvents(DateTime startDateOfWeek)
    {
        return ExecutionDays.Select(executionDay =>
        {
            var targetDate = CalculateDateTime(startDateOfWeek, executionDay.Day, executionDay.Time);
            return new EventInstance(EventId.New(),
                targetDate,
                Id,
                EventDuration,
                EventStatus.Scheduled);
        });
    }

    /// <summary>
    /// Вычисляет конкретное значение <see cref="DateTime"/> на основе заданной начальной даты, целевого дня недели и времени суток.
    /// </summary>
    /// <param name="startDate">Дата, с которой начинается расчет.</param>
    /// <param name="targetDay">Целевой день недели, который необходимо вычислить.</param>
    /// <param name="time">Время суток, которое необходимо применить к полученной дате.</param>
    /// <returns>
    /// Значение <see cref="DateTime"/>, представляющее указанную дату и время, рассчитанное относительно начальной даты.
    /// </returns>
    /// <remarks>
    /// Этот метод вычисляет ближайшее наступление указанного целевого дня недели, начиная с заданной даты.
    /// Если целевой день совпадает с днем начальной даты, результат будет соответствовать этой же дате с заданным временем.
    /// Временной компонент добавляется с использованием параметра <see cref="TimeSpan"/>.
    /// </remarks>
    private static DateTime CalculateDateTime(DateTime startDate, DayOfWeek targetDay, TimeSpan time)
    {
        var daysDifference = ((int)targetDay - (int)startDate.DayOfWeek + 7) % 7;
        var targetDate = startDate.AddDays(daysDifference);
        return targetDate.Date + time;
    }
}