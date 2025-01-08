using Microsoft.Extensions.DependencyInjection;
using SachkovTech.Core.Database;
using UserProgressService.Application.Interfaces;
using UserProgressService.Infrastructure.Repositories;

namespace UserProgressService.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services)
    {
        services
            .AddRepositories()
            .AddDatabase();

        return services;
    }

    private static IServiceCollection AddDatabase(this IServiceCollection services)
    {
        services.AddScoped<AppDbContext>();
        services.AddScoped<IDbContext, AppDbContext>();
        services.AddScoped<IUnitOfWork, UnitOfWork>();

        return services;
    }

    private static IServiceCollection AddRepositories(this IServiceCollection services)
    {
        services.AddScoped<IAchievementRepository, AchievementRepository>();

        return services;
    }
}