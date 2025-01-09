using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Quartz;
using ScheduleService.Application;
using ScheduleService.Infrastructure.Database;
using ScheduleService.Infrastructure.QuartzManagement.Abstratction;
using ScheduleService.Infrastructure.QuartzManagement.Configuration;
using ScheduleService.Infrastructure.QuartzManagement.Extensions;
using ScheduleService.Infrastructure.QuartzManagement.Jobs;

namespace ScheduleService.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        return services.AddScoped<ApplicationDbContext>()
                    .ConfigureQuartz(configuration)
                    .AddScoped<EventJobFactory>()
                    .AddScoped<ISendToBus, SendToBus>()
                    .AddScoped<IDateTimeProvider, DateTimeProvider>();
    }

    public static IServiceCollection ConfigureQuartz(this
        IServiceCollection services, IConfiguration configuration)
    {
        return services.CreateQuartzTables(configuration)
                    .Configure<QuartzOptions>(options =>
                    {
                        configuration.GetSection("Quartz");
                        options.Scheduling.IgnoreDuplicates = true;
                        options.Scheduling.OverWriteExistingData = true;
                    })
                    .AddQuartz(q =>
                    {
                        q.UsePersistentStore(store =>
                        {
                            store.UsePostgres(dbConfig =>
                            {
                                dbConfig.ConnectionString =
                                    configuration.GetConnectionString(ApplicationDbContext.DATABASE)!;
                                dbConfig.TablePrefix = "qrtz_";
                            });
                            store.PerformSchemaValidation = false;
                            store.UseBinarySerializer();
                            store.UseClustering();
                        });
                    })
                    .AddQuartzHostedService(options =>
                    {
                        options.WaitForJobsToComplete = true;
                    });
    }
}
