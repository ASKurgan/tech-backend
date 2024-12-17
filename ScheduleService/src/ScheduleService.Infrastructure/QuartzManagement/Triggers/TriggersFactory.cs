using Quartz;

namespace ScheduleService.Infrastructure.QuartzManagement.Triggers;
public class TriggersFactory
{
    public static TriggerKey GetKey(string triggerName, string groupName) =>
        new(triggerName, groupName);

    public static ITrigger CreateWithCron(
        TriggerKey key,
        string cron,
        DateTime startDate,
        DateTime endDate)
    {
        return TriggerBuilder.Create()
            .WithIdentity(key)
            .WithCronSchedule(cron)
            .StartAt(startDate)
            .EndAt(endDate)
            .Build();
    }

    public static ITrigger CreateWithCron(
        string triggerName,
        string groupName,
        string cron,
        DateTime startdate,
        DateTime stopDate)
    {
        var key = GetKey(triggerName, groupName);

        return TriggerBuilder.Create()
            .WithIdentity(key)
            .WithCronSchedule(cron)
            .StartAt(startdate)
            .EndAt(stopDate)
            .Build();
    }
}
