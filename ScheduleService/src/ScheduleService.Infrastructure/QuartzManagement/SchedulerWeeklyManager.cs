using Quartz;
using ScheduleService.Domain.ValueObjects;
using ScheduleService.Infrastructure.QuartzManagement.Jobs;
using ScheduleService.Infrastructure.QuartzManagement.Triggers;

namespace ScheduleService.Infrastructure.QuartzManagement;

public class SchedulerWeeklyManager(ISchedulerFactory schedulerFactory)
{
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

    public async Task SchedulerJob(
        List<EventTime> schedulerDates,
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
