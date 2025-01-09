using Quartz;
using ScheduleService.Infrastructure.QuartzManagement.Jobs;
using ScheduleService.Infrastructure.QuartzManagement.Triggers;

namespace ScheduleService.Infrastructure.QuartzManagement;

public class SchedulerMonthlyManager(ISchedulerFactory schedulerFactory)
{
    /// <summary>
    ///    Метод для планирования ежемесячной задачи.
    /// </summary>
    /// <param name="schedulerDate">день месяца и время выполнения задачи</param>
    /// <param name="startDate">дата начала планирования задачи</param>
    /// <param name="endDate">дата завершения планирования задачи, 
    /// разница между startDate и endDate определяет сколько месяцев каждая задача будет повторяться</param>
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
        var schedulerInstance = await schedulerFactory.GetScheduler();

        var jobKey = JobsFactory.GetKey(jobId, Constants.DEFAULT_GROUP);

        var job = JobsFactory.Create(jobKey, jobDescription);

        var cronExpression = DateToCronConvertor.GetMonthlyCron(schedulerDate);

        var trigger = TriggersFactory.CreateWithCron(
            triggerId,
            Constants.DEFAULT_GROUP,
            cronExpression,
            startDate,
            endDate);

        await schedulerInstance.ScheduleJob(job, trigger, cancellationToken);
    }

    /// <summary>
    ///    Метод для планирования ежемесячной задачи, выполняющейся более одного раза в течение месяца.
    /// </summary>
    /// <param name="schedulerDate">дни месяца и время выполнения задачи</param>
    /// <param name="startDate">дата начала планирования задачи</param>
    /// <param name="endDate">дата завершения планирования задачи, 
    /// разница между startDate и endDate определяет сколько месяцев каждая задача будет повторяться</param>
    /// <param name="jobId">Id выполняемой задачи</param>
    /// <param name="triggerId">Id объекта-триггера, который обеспечивает выполнение задачи по времени</param>
    /// <param name="jobDescription">краткое описание задачи</param>
    /// <param name="cancellationToken">токен отмены выполнения асинхронной операции</param>
    /// <returns>Возвращает объект Task.</returns>
    public async Task SchedulerJob(
        List<DateTime> schedulerDate,
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

        foreach (var date in schedulerDate)
        {
            var cronExpression = DateToCronConvertor.GetMonthlyCron(date);

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