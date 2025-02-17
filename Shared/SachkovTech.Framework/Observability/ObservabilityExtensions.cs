using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Npgsql;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;

namespace SachkovTech.Framework.Observability;

public static class ObservabilityExtensions
{
    public static IServiceCollection AddObservability(
        this IServiceCollection services,
        IConfiguration configuration,
        string[] meterNames,
        string[] sourceNames)
    {
        var observabilityOptions = configuration.GetSection(ObservabilityOptions.OBSERVABILITY)
            .Get<ObservabilityOptions>() ?? throw new ArgumentNullException(nameof(ObservabilityOptions));

        services.AddOpenTelemetry()
            .ConfigureResource(resource => resource.AddService(observabilityOptions.ServiceName))
            .WithMetrics(metrics => metrics
                .SetResourceBuilder(ResourceBuilder.CreateDefault().AddService(observabilityOptions.ServiceName))
                .AddAspNetCoreInstrumentation()
                .AddHttpClientInstrumentation()
                .AddRuntimeInstrumentation()
                .AddProcessInstrumentation()
                .AddPrometheusExporter()
                .AddMeter(meterNames))
            .WithTracing(tracing => tracing
                .AddAspNetCoreInstrumentation()
                .AddHttpClientInstrumentation()
                .AddNpgsql()
                .AddSource(sourceNames)
                .AddOtlpExporter(c => c.Endpoint = new Uri(observabilityOptions.OltpEndpoint)));

        return services;
    }
}