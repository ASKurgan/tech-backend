using Quartz;
using ScheduleService.Infrastructure.QuartzManagement.Abstratction;

namespace ScheduleService.Infrastructure.QuartzManagement.Jobs;

public class EventJob : IJob
{
    private ISendToBus _sendToBus;
    public EventJob(ISendToBus sendToBus)
    {
        _sendToBus = sendToBus;
    }

    public async Task Execute(IJobExecutionContext context)
    {
        //for example
        await Task.Run(() =>
            _sendToBus.SendToBus($"Job executed at: {DateTime.Now:G}"));
    }
}
