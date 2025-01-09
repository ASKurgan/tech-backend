using Quartz;

namespace ScheduleService.Infrastructure.QuartzManagement.Triggers;
public class TriggersFactory
{
    public static TriggerKey GetKey(string triggerName, string groupName) =>
        new(triggerName, groupName);

    /// <summary>
    ///    Метод для создания триггера, срабатывающего в заданное время.
    /// </summary>
    /// <param name="key">Идентификатор триггера</param>
    /// <param name="cron">крон-выражение для планирования задачи</param>
    /// <param name="startDate">дата начала планирования задачи</param>
    /// <param name="endDate">дата завершения планирования задачи</param>   
    /// <returns>Возвращает объект триггер.</returns>
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

    /// <summary>
    ///    Метод для создания триггера, срабатывающего в заданное время.
    /// </summary>
    /// <param name="triggerName">Имя триггера триггера</param>
    /// <param name="groupName">Имя группы, которой принадлежит триггер</param>
    /// <param name="cron">крон-выражение для планирования задачи</param>
    /// <param name="startDate">дата начала планирования задачи</param>
    /// <param name="endDate">дата завершения планирования задачи</param>   
    /// <returns>Возвращает объект триггер.</returns>
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
