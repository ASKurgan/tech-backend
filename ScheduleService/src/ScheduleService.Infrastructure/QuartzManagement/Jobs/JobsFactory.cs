using Quartz;

namespace ScheduleService.Infrastructure.QuartzManagement.Jobs;

public class JobsFactory
{
    public static JobKey GetKey(string JobId, string group) =>
        new(JobId, group);

    public static IJobDetail Create(JobKey key, string jobDescription)
    {
        return JobBuilder.Create<EventJob>()
            .WithIdentity(key)
            .WithDescription(jobDescription)
            .StoreDurably()
            .Build();
    }

    public static IJobDetail Create(string jobName, string group, string jobDescription)
    {
        var key = GetKey(jobName, group);

        return JobBuilder.Create<EventJob>()
            .WithIdentity(key)
            .WithDescription(jobDescription)
            .StoreDurably()
            .Build();
    }
}