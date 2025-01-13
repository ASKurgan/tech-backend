using System.Reflection;
using Elastic.CommonSchema.Serilog;
using Elastic.Ingest.Elasticsearch;
using Elastic.Ingest.Elasticsearch.DataStreams;
using Elastic.Serilog.Sinks;
using FileService.Communication;
using FluentValidation;
using Microsoft.OpenApi.Models;
using Npgsql;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using SachkovTech.Core.Abstractions;
using SachkovTech.Core.Caching;
using SachkovTech.Framework.Authorization;
using SachkovTech.Issues.Application;
using SachkovTech.Issues.Infrastructure;
using Serilog;
using Serilog.Events;

namespace SachkovTech.Web;

public static class DependencyInjection
{
    public static void AddProgramDependencies(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddControllers();

        services.AddEndpointsApiExplorer()
            .AddSwaggerConfiguration()
            .AddLogging(configuration)
            .AddApplicationLayers()
            .AddFramework()
            .AddIssuesModule(configuration)
            .AddAuthServices(configuration)
            .AddAppMetrics(configuration)
            .AddDistributedCache(configuration);
    }

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

    private static IServiceCollection AddIssuesModule(
        this IServiceCollection services, IConfiguration configuration)
    {
        services.AddIssuesApplication()
            .AddIssuesInfrastructure(configuration);

        services.AddFileHttpCommunication(configuration);

        return services;
    }

    private static IServiceCollection AddAppMetrics(
        this IServiceCollection services, IConfiguration configuration)
    {
        services.AddOpenTelemetry()
            .ConfigureResource(resource => resource.AddService("SachkovTech.Issues.Api"))
            .WithMetrics(metrics => metrics
                .SetResourceBuilder(ResourceBuilder.CreateDefault().AddService("SachkovTech.Issues.Api"))
                .AddAspNetCoreInstrumentation()
                .AddHttpClientInstrumentation()
                .AddRuntimeInstrumentation()
                .AddProcessInstrumentation()
                .AddPrometheusExporter())
            .WithTracing(tracing => tracing
                .AddAspNetCoreInstrumentation()
                .AddHttpClientInstrumentation()
                .AddNpgsql()
                .AddSource(MassTransit.Logging.DiagnosticHeaders.DefaultListenerName)
                .AddOtlpExporter(c => c.Endpoint = new Uri("http://localhost:4317")));

        return services;
    }

    private static IServiceCollection AddLogging(
        this IServiceCollection services, IConfiguration configuration)
    {
        string indexFormat =
            $"{Assembly.GetExecutingAssembly().GetName().Name?.ToLower().Replace(".", "-")}-{DateTime.UtcNow:dd-MM-yyyy}";

        Log.Logger = new LoggerConfiguration()
            .Enrich.FromLogContext()
            .WriteTo.Console()
            .WriteTo.Debug()
            .WriteTo.Elasticsearch(
                [new Uri("http://localhost:9200")],
                options =>
                {
                    options.DataStream = new DataStreamName(indexFormat);
                    options.TextFormatting = new EcsTextFormatterConfiguration();
                    options.BootstrapMethod = BootstrapMethod.Silent;
                })
            .MinimumLevel.Override("Microsoft.AspNetCore.Hosting", LogEventLevel.Warning)
            .MinimumLevel.Override("Microsoft.AspNetCore.Mvc", LogEventLevel.Warning)
            .MinimumLevel.Override("Microsoft.AspNetCore.Routing", LogEventLevel.Warning)
            .CreateLogger();

        services.AddSerilog();

        return services;
    }

    private static IServiceCollection AddFramework(this IServiceCollection services)
    {
        services.AddHttpContextAccessor()
            .AddScoped<UserScopedData>();

        return services;
    }

    private static IServiceCollection AddApplicationLayers(this IServiceCollection services)
    {
        var assemblies = new[] { typeof(SachkovTech.Issues.Application.DependencyInjection).Assembly, };

        services.Scan(scan => scan.FromAssemblies(assemblies)
            .AddClasses(classes => classes
                .AssignableToAny(typeof(ICommandHandler<,>), typeof(ICommandHandler<>)))
            .AsSelfWithInterfaces()
            .WithScopedLifetime());

        services.Scan(scan => scan.FromAssemblies(assemblies)
            .AddClasses(classes => classes
                .AssignableToAny(typeof(IQueryHandler<,>), typeof(IQueryHandlerWithResult<,>)))
            .AsSelfWithInterfaces()
            .WithScopedLifetime());

        services.AddValidatorsFromAssemblies(assemblies);
        return services;
    }

    private static IServiceCollection AddSwaggerConfiguration(this IServiceCollection services)
    {
        services.AddSwaggerGen(c =>
        {
            c.SwaggerDoc("v1", new OpenApiInfo { Title = "My API", Version = "v1" });
            c.AddSecurityDefinition(
                "Bearer",
                new OpenApiSecurityScheme
                {
                    In = ParameterLocation.Header,
                    Description = "Please insert JWT with Bearer into field",
                    Name = "Authorization",
                    Type = SecuritySchemeType.ApiKey,
                });
            c.AddSecurityRequirement(new OpenApiSecurityRequirement
            {
                {
                    new OpenApiSecurityScheme
                    {
                        Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "Bearer", },
                    },
                    []
                },
            });
        });
        return services;
    }
}