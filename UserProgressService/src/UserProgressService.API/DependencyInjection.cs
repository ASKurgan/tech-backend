using Serilog;
using Serilog.Events;
using UserProgressService.Application;
using UserProgressService.Infrastructure;

namespace UserProgressService.API;

public static class DependencyInjection
{
    public static void AddProgramDependencies(this IServiceCollection services)
    {
        services
            .AddInfrastructure()
            .AddApplication();
    }
    
    public static IServiceCollection AddLogging(
        this IServiceCollection services, IConfiguration configuration)
    {
        Log.Logger = new LoggerConfiguration()
            .WriteTo.Console()
            .WriteTo.Debug()
            .WriteTo.Seq(configuration.GetConnectionString("Seq")
                         ?? throw new ArgumentNullException("Seq"))
            .MinimumLevel.Override("Microsoft.AspNetCore.Hosting", LogEventLevel.Warning)
            .MinimumLevel.Override("Microsoft.AspNetCore.Mvc", LogEventLevel.Warning)
            .MinimumLevel.Override("Microsoft.AspNetCore.Routing", LogEventLevel.Warning)
            .CreateLogger();

        services.AddSerilog();

        return services;
    }
}