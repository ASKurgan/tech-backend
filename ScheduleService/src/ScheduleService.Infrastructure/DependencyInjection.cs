using Microsoft.Extensions.DependencyInjection;
using ScheduleService.Application;

namespace ScheduleService.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services)
    {
        return services.AddScoped<IDateTimeProvider, DateTimeProvider>();
    }
}
