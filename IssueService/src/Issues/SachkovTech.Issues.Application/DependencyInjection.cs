using Microsoft.Extensions.DependencyInjection;

namespace SachkovTech.Issues.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddIssuesApplication(this IServiceCollection services)
    {
        services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(DependencyInjection).Assembly));

        return services;
    }
}