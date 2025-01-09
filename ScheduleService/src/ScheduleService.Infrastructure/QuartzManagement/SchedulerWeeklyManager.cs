using Quartz;
using ScheduleService.Domain.ValueObjects;
using ScheduleService.Infrastructure.QuartzManagement.Jobs;
using ScheduleService.Infrastructure.QuartzManagement.Triggers;

namespace ScheduleService.Infrastructure.QuartzManagement;

public class SchedulerWeeklyManager(ISchedulerFactory schedulerFactory)
{
    /// <summary>
    ///    Метод для планирования еженедельной задачи.
    /// </summary>
    /// <param name="schedulerDate">время, в которое будет выполнятья задача</param>
    /// <param name="dayOfWeek">день недели, в который будет выполняться задача</param>
    /// <param name="startDate">дата начала планирования задачи</param>
    /// <param name="endDate">дата завершения планирования задачи, 
    /// разница между startDate и endDate определяет сколько недель каждая задача будет повторяться</param>
    /// <param name="jobId">Id выполняемой задачи</param>
    /// <param name="triggerId">Id объекта-триггера, который обеспечивает выполнение задачи по времени</param>
    /// <param name="jobDescription">краткое описание задачи</param>
    /// <param name="cancellationToken">токен отмены выполнения асинхронной операции</param>
    /// <returns>Возвращает объект Task.</returns>
    public async Task SchedulerJob(
        DateTime schedulerDate,
        DayOfWeek dayOfWeek,
        DateTime startDate,
        DateTime endDate,
        string jobId,
        string triggerId,
        string jobDescription,
        CancellationToken cancellationToken)
    {
        var schedulerInstance = await schedulerFactory.GetScheduler(cancellationToken);

        var jobKey = JobsFactory.GetKey(jobId, Constants.DEFAULT_GROUP);

        var job = JobsFactory.Create(jobKey, jobDescription);

        var cronExpression = DateToCronConvertor.GetWeeklyCron(schedulerDate, dayOfWeek);

        var trigger = TriggersFactory.CreateWithCron(
            triggerId,
            Constants.DEFAULT_GROUP,
            cronExpression,
            startDate,
            endDate);

        await schedulerInstance.ScheduleJob(job, trigger, cancellationToken);
    }

    /// <summary>
    ///    Метод для планирования еженедельной задачи.
    /// </summary>
    /// <param name="schedulerDate">продолжительность времени до начала выполнения задачи</param>
    /// <param name="dayOfWeek">день недели, в который будет выполняться задача</param>
    /// <param name="startDate">дата начала планирования задачи</param>
    /// <param name="endDate">дата завершения планирования задачи, 
    /// разница между startDate и endDate определяет сколько недель каждая задача будет повторяться</param>
    /// <param name="jobId">Id выполняемой задачи</param>
    /// <param name="triggerId">Id объекта-триггера, который обеспечивает выполнение задачи по времени</param>
    /// <param name="jobDescription">краткое описание задачи</param>
    /// <param name="cancellationToken">токен отмены выполнения асинхронной операции</param>
    /// <returns>Возвращает объект Task.</returns>
    public async Task SchedulerJob(
        TimeSpan schedulerDate,
        DayOfWeek dayOfWeek,
        DateTime startDate,
        DateTime endDate,
        string jobId,
        string triggerId,
        string jobDescription,
        CancellationToken cancellationToken)
    {
        var schedulerInstance = await schedulerFactory.GetScheduler(cancellationToken);

        var jobKey = JobsFactory.GetKey(jobId, Constants.DEFAULT_GROUP);

        var job = JobsFactory.Create(jobKey, jobDescription);

        var date = Constants.START_DAY_TIME;

        var cronExpression = DateToCronConvertor.GetWeeklyCron(date.Add(schedulerDate), dayOfWeek);

        var trigger = TriggersFactory.CreateWithCron(
            triggerId,
            Constants.DEFAULT_GROUP,
            cronExpression,
            startDate,
            endDate);

        await schedulerInstance.ScheduleJob(job, trigger, cancellationToken);
    }

    /// <summary>
    ///    Метод для планирования еженедельной задачи, выполняющейся более одного раза в течение недели.
    /// </summary>
    /// <param name="schedulerDates">дни недели и время выполнения задачи</param>
    /// <param name="startDate">дата начала планирования задачи</param>
    /// <param name="endDate">дата завершения планирования задачи, 
    /// разница между startDate и endDate определяет сколько недель каждая задача будет повторяться</param>
    /// <param name="jobId">Id выполняемой задачи</param>
    /// <param name="triggerId">Id объекта-триггера, который обеспечивает выполнение задачи по времени</param>
    /// <param name="jobDescription">краткое описание задачи</param>
    /// <param name="cancellationToken">токен отмены выполнения асинхронной операции</param>
    /// <returns>Возвращает объект Task.</returns>
    public async Task SchedulerJob(
        List<EventTime> schedulerDates,        
        DateTime startDate,
        DateTime endDate,
        string jobId,
        string triggerId,
        string jobDescription,
        CancellationToken cancellationToken)
    {
        var schedulerInstance = await schedulerFactory.GetScheduler(cancellationToken);

        var jobKey = JobsFactory.GetKey(jobId, Constants.DEFAULT_GROUP);

        var job = JobsFactory.Create(jobKey, jobDescription);

        foreach(var dateItem in schedulerDates)
        {
            var date = Constants.START_DAY_TIME;

            var cronExpression = DateToCronConvertor.GetWeeklyCron(
                date.Add(dateItem.Time), 
                dateItem.Day);

            var trigger = TriggersFactory.CreateWithCron(
                triggerId,
                Constants.DEFAULT_GROUP,
                cronExpression,
                startDate,
                endDate);

            await schedulerInstance.ScheduleJob(job, trigger, cancellationToken);
        }       
    }
}
