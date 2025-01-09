using Quartz;
using ScheduleService.Infrastructure.QuartzManagement.Jobs;
using ScheduleService.Infrastructure.QuartzManagement.Triggers;

namespace ScheduleService.Infrastructure.QuartzManagement;

public class SchedulerDailyManager(ISchedulerFactory schedulerFactory)
{
    /// <summary>
    ///    Метод для планирования ежедневной задачи.
    /// </summary>
    /// <param name="schedulerDate">время выполнения задачи</param>
    /// <param name="startDate">дата начала планирования задачи</param>
    /// <param name="endDate">дата завершения планирования задачи, 
    /// разница между startDate и endDate определяет сколько дней задача будет повторяться</param>
    /// <param name="jobId">Id выполняемой задачи</param>
    /// <param name="triggerId">Id объекта-триггера, который обеспечивает выполнение задачи по времени</param>
    /// <param name="jobDescription">краткое описание задачи</param>
    /// <param name="cancellationToken">токен отмены выполнения асинхронной операции</param>
    /// <returns>Возвращает объект Task.</returns>
    public async Task SchedulerJob(
        DateTime schedulerDate,
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

        var cronExpression = DateToCronConvertor.GetDailyCron(schedulerDate);

        var trigger = TriggersFactory.CreateWithCron(
            triggerId,
            Constants.DEFAULT_GROUP,
            cronExpression,
            startDate,
            endDate);

        await schedulerInstance.ScheduleJob(job, trigger, cancellationToken);
    }

    /// <summary>
    ///    Метод для планирования ежедневной задачи.
    /// </summary>
    /// <param name="schedulerDate">период времени до начала выполнения задачи</param>
    /// <param name="startDate">дата начала планирования задачи</param>
    /// <param name="endDate">дата завершения планирования задачи, 
    /// разница между startDate и endDate определяет сколько дней задача будет повторяться</param>
    /// <param name="jobId">Id выполняемой задачи</param>
    /// <param name="triggerId">Id объекта-триггера, который обеспечивает выполнение задачи по времени</param>
    /// <param name="jobDescription">краткое описание задачи</param>
    /// <param name="cancellationToken">токен отмены выполнения асинхронной операции</param>
    /// <returns>Возвращает объект Task.</returns>
    public async Task SchedulerJob(
        TimeSpan spanDuration,
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

        var cronExpression = DateToCronConvertor.GetDailyCron(date.Add(spanDuration));

        var trigger = TriggersFactory.CreateWithCron(
            triggerId,
            Constants.DEFAULT_GROUP,
            cronExpression,
            startDate,
            endDate);

        await schedulerInstance.ScheduleJob(job, trigger, cancellationToken);
    }

    /// <summary>
    ///    Метод для планирования ежедневных задач.
    /// </summary>
    /// <param name="schedulerDates">список времен выполнения задачи</param>
    /// <param name="startDate">дата начала планирования задачи</param>
    /// <param name="endDate">дата завершения планирования задачи, 
    /// разница между startDate и endDate определяет сколько дней каждая задача будет повторяться</param>
    /// <param name="jobId">Id выполняемой задачи</param>
    /// <param name="triggerId">Id объекта-триггера, который обеспечивает выполнение задачи по времени</param>
    /// <param name="jobDescription">краткое описание задачи</param>
    /// <param name="cancellationToken">токен отмены выполнения асинхронной операции</param>
    /// <returns>Возвращает объект Task.</returns>
    public async Task SchedulerJob(
        List<DateTime> schedulerDates,
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

        foreach (var date in schedulerDates)
        {
            var cronExpression = DateToCronConvertor.GetDailyCron(date);

            var trigger = TriggersFactory.CreateWithCron(
                triggerId,
                Constants.DEFAULT_GROUP,
                cronExpression,
                startDate,
                endDate);

            await schedulerInstance.ScheduleJob(job, trigger, cancellationToken);
        }
    }

    /// <summary>
    ///    Метод для планирования ежедневных задач.
    /// </summary>
    /// <param name="schedulerDates">список периодов времени до начала выполнения задачи</param>
    /// <param name="startDate">дата начала планирования задачи</param>
    /// <param name="endDate">дата завершения планирования задачи, 
    /// разница между startDate и endDate определяет сколько дней каждая задача будет повторяться</param>
    /// <param name="jobId">Id выполняемой задачи</param>
    /// <param name="triggerId">Id объекта-триггера, который обеспечивает выполнение задачи по времени</param>
    /// <param name="jobDescription">краткое описание задачи</param>
    /// <param name="cancellationToken">токен отмены выполнения асинхронной операции</param>
    /// <returns>Возвращает объект Task.</returns>
    public async Task SchedulerJob(
        List<TimeSpan> schedulerDates,
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

        foreach (var spanDuration in schedulerDates)
        {
            var date = Constants.START_DAY_TIME;

            var cronExpression = DateToCronConvertor.GetDailyCron(date.Add(spanDuration));

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
