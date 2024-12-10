using CSharpFunctionalExtensions;
using ScheduleService.Domain.Entities;
using ScheduleService.Domain.Enums;
using ScheduleService.Domain.Ids;
using ScheduleService.Domain.Shared;
using ScheduleService.Domain.ValueObjects;
using SharedKernel;

namespace ScheduleService.Domain.TypeSchedules;

public sealed class MonthlySchedule : Schedule
{
    private MonthlySchedule(ScheduleId id) : base(id)
    {
    }

    private MonthlySchedule(
        ScheduleId id,
        DateTime startDate,
        DateTime endDate,
        TimeSpan eventDuration,
        List<DateTime> executionDays,
        Title title,
        Description description,
        int repeatInterval,
        bool isAutomaticRenewal) : base(id, title, description, startDate, endDate, eventDuration, isAutomaticRenewal)
    {
        RepeatInterval = repeatInterval;
        _executionDays = executionDays;
    }

    public int RepeatInterval { get; }

    private readonly List<DateTime> _executionDays = [];

    public IReadOnlyList<DateTime> ExecutionDays => _executionDays;

    /// <summary>
    /// Создает экземпляр MonthlySchedule с заданными параметрами.
    /// Выполняет проверку входных данных и генерирует запланированные события.
    /// </summary>
    /// <param name="id">Идентификатор расписания.</param>
    /// <param name="startDate">Дата начала расписания.</param>
    /// <param name="endDate">Дата окончания расписания.</param>
    /// <param name="executionDays">Список дней и времени выполнения.</param>
    /// <param name="duration">Время начала для всех событий.</param>
    /// <param name="title">Заголовок расписания.</param>
    /// <param name="description">Описание расписания.</param>
    /// <param name="repeatInterval">Интервал повторения (в месяцах).</param>
    /// <param name="isAutomaticRenewal">Флаг автоматического продления расписания.</param>
    /// <returns>Результат создания расписания (успешный или с ошибкой).</returns>
    public static Result<MonthlySchedule, Error> Create(
        ScheduleId id,
        DateTime startDate,
        DateTime endDate,
        List<DateTime> executionDays,
        TimeSpan duration,
        Title title,
        Description description,
        int repeatInterval,
        bool isAutomaticRenewal)
    {
        if (!executionDays.Any())
            return Errors.General.ValueIsInvalid("executionTimeList");

        if (endDate < startDate)
            return Errors.General.ValueIsInvalid("endDate");

        if (repeatInterval < 0)
            return Errors.General.ValueIsInvalid("repeatInterval");

        // Создаем объект расписания
        var schedule = new MonthlySchedule(id,
            startDate,
            endDate,
            duration,
            executionDays,
            title,
            description,
            repeatInterval,
            isAutomaticRenewal);

        var isEventsAdded = schedule.AddEvents();
        if (isEventsAdded.IsFailure)
            return isEventsAdded.Error;

        return schedule;
    }

    /// <summary>
    /// Продление расписания на месяц
    /// </summary>
    /// <returns>Результат, что продление прошло либо ошибка</returns>
    public UnitResult<Error> ExtendEvents()
    {
        if (!IsAutomaticRenewal)
            return Errors.Schedule.RenewalIsDisabled();
        
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

    private IEnumerable<EventInstance> ExtendSchedule(DateTime startDate)
    {
        return ExecutionDays.Select(executionDay => CalculateMonthlyDate(startDate, 0, executionDay))
            .Select(targetDate =>
                new EventInstance(EventId.New(),
                    targetDate,
                    Id,
                    EventDuration,
                    EventStatus.Scheduled));
    }

    /// <summary>
    /// Генерирует запланированные события на основе дней выполнения и интервала повторений.
    /// </summary>
    private UnitResult<Error> AddEvents()
    {
        var result = Enumerable
            .Range(0, RepeatInterval)
            .SelectMany(i => ExecutionDays.Select(executionDay =>
            {
                var targetDate = CalculateMonthlyDate(StartDate, i, executionDay);
                return new EventInstance(EventId.New(), targetDate, Id, EventDuration, EventStatus.Scheduled);
            }))
            .Select(AddEvent)
            .FirstOrDefault(isEventAdded => isEventAdded.IsFailure);

        return result.IsFailure ? result.Error : UnitResult.Success<Error>();
    }

    /// <summary>
    /// Рассчитывает корректную дату выполнения в конкретный месяц с учетом длины месяца.
    /// </summary>
    /// <param name="startDate">Дата начала расписания.</param>
    /// <param name="monthOffset">Смещение по месяцам от начальной даты.</param>
    /// <param name="executionDay">День выполнения (с датой и временем).</param>
    /// <returns>Рассчитанная дата выполнения события.</returns>
    public static DateTime CalculateMonthlyDate(DateTime startDate, int monthOffset, DateTime executionDay)
    {
        // Добавляем месячный интервал
        var monthDate = startDate.AddMonths(monthOffset);

        // Устанавливаем день из executionDay (учитываем случаи, когда день отсутствует в месяце)
        var day = Math.Min(executionDay.Day, DateTime.DaysInMonth(monthDate.Year, monthDate.Month));

        return new DateTime(monthDate.Year, monthDate.Month, day, executionDay.Hour, executionDay.Minute,
            executionDay.Second);
    }
}
