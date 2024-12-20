using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace SachkovTech.Core.Caching;

public static class CacheExtensions
{
    private static IServiceCollection AddDistributedCache(
        this IServiceCollection services, IConfiguration configuration)
    {
        services.AddStackExchangeRedisCache(options =>
        {
            string connection = configuration.GetConnectionString("Redis")
                                ?? throw new ArgumentNullException(nameof(connection));

            options.Configuration = connection;
        });

        services.AddSingleton<ICacheService, DistributedCacheService>();

        return services;
    }
}