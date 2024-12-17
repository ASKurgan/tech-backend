using Quartz;
using ScheduleService.Infrastructure.QuartzManagement.Jobs;
using ScheduleService.Infrastructure.QuartzManagement.Triggers;

namespace ScheduleService.Infrastructure.QuartzManagement;

public class SchedulerMonthlyManager(ISchedulerFactory schedulerFactory)
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