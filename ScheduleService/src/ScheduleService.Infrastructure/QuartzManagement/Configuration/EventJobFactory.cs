using Microsoft.Extensions.DependencyInjection;
using Quartz;
using Quartz.Spi;

namespace ScheduleService.Infrastructure.QuartzManagement.Configuration;

public class EventJobFactory : IJobFactory
{
    private readonly IServiceScopeFactory _scopeFactory;

    public EventJobFactory(IServiceScopeFactory scopeFactory)
    {
        _scopeFactory = scopeFactory;
    }

    //result needs to check null
    public IJob? NewJob(TriggerFiredBundle bundle, IScheduler scheduler)
    {
        using var scope = _scopeFactory.CreateScope();

        return ActivatorUtilities.CreateInstance(
            scope.ServiceProvider,
            bundle.JobDetail.JobType) as IJob;
    }

    public void ReturnJob(IJob job)
    {
        (job as IDisposable)?.Dispose();
    }
}
