using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Quartz;
using ScheduleService.Application;
using ScheduleService.Infrastructure.Database;
using ScheduleService.Infrastructure.QuartzManagement.Abstratction;
using ScheduleService.Infrastructure.QuartzManagement.Configuration;
using ScheduleService.Infrastructure.QuartzManagement.Jobs;

namespace ScheduleService.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.ConfigureQuartz(configuration);
        services.AddScoped<EventJobFactory>();
        services.AddScoped<ISendToBus, SendToBus>();
        return services.AddScoped<IDateTimeProvider, DateTimeProvider>();
    }

    public static IServiceCollection ConfigureQuartz(this
        IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<QuartzOptions>(options =>
        {
            configuration.GetSection("Quartz");
            options.Scheduling.IgnoreDuplicates = true;
            options.Scheduling.OverWriteExistingData = true;
        });

        services.AddQuartz(q =>
        {
            q.UsePersistentStore(store =>
            {
                store.UsePostgres(dbConfig =>
                {
                    dbConfig.ConnectionString =
                        configuration.GetConnectionString(ApplicationDbContext.DATABASE)!;
                    dbConfig.TablePrefix = "qrtz_";
                });
                store.UseBinarySerializer();
                store.UseClustering();
            });
        });

        return services.AddQuartzHostedService(options =>
        {
            options.WaitForJobsToComplete = true;
        });
    }
}
