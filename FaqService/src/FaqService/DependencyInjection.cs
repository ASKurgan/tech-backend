using FaqService.Infrastructure;
using FaqService.Infrastructure.Repositories;
using Nest;
using SachkovTech.Framework.Logging;

namespace FaqService;

public static class DependencyInjection
{
    public static IServiceCollection AddLogging(
        this IServiceCollection services, IConfiguration configuration)
    {
        services.AddApplicationLogging(configuration);

        return services;
    }

    public static IServiceCollection AddDbContext(this IServiceCollection services)
    {
        services.AddScoped<ApplicationDbContext>();

        return services;
    }

    public static IServiceCollection AddElasticSearch(this IServiceCollection services, IConfiguration configuration)
    {
        var elasticSearchSettings = configuration.GetConnectionString("ElasticSearch");
        var settings = new ConnectionSettings(new Uri(elasticSearchSettings!))
            .DefaultIndex("posts");

        var client = new ElasticClient(settings);

        services.AddSingleton<IElasticClient>(client);

        return services;
    }

    public static IServiceCollection AddRepositories(this IServiceCollection services)
    {
        services.AddScoped<SearchRepository>();
        services.AddScoped<UnitOfWork>();

        return services;
    }
}