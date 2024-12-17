using Quartz;
using ScheduleService.Infrastructure.QuartzManagement.Jobs;
using ScheduleService.Infrastructure.QuartzManagement.Triggers;

namespace ScheduleService.Infrastructure.QuartzManagement;

public class SchedulerDailyManager(ISchedulerFactory schedulerFactory)
{
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
