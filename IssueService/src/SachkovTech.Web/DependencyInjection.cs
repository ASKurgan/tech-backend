using System.Reflection;
using FileService.Communication;
using FluentValidation;
using MassTransit.Logging;
using MassTransit.Monitoring;
using Microsoft.OpenApi.Models;
using SachkovTech.Core.Abstractions;
using SachkovTech.Core.Caching;
using SachkovTech.Framework.Authorization;
using SachkovTech.Framework.Logging;
using SachkovTech.Framework.Observability;
using SachkovTech.Issues.Application;
using SachkovTech.Issues.Infrastructure;

namespace SachkovTech.Web;

public static class DependencyInjection
{
    public static void AddProgramDependencies(this IServiceCollection services, IConfiguration configuration)
    {
        var assemblies = new[]
        {
            typeof(SachkovTech.Issues.Application.DependencyInjection).Assembly,
        };

        services.AddControllers();

        services
            .AddApplication()
            .AddInfrastructure(configuration)
            .AddFramework(configuration, assemblies);
    }

    private static IServiceCollection AddFramework(
        this IServiceCollection services,
        IConfiguration configuration,
        params Assembly[] assemblies)
    {
        services.AddEndpointsApiExplorer()
            .AddApplicationLogging(configuration)
            .AddValidatorsFromAssemblies(assemblies)
            .AddHandlers(assemblies)
            .AddSwaggerConfiguration()
            .AddAuthServices(configuration)
            .AddDistributedCache(configuration)
            .AddObservability(configuration, [InstrumentationOptions.MeterName], [DiagnosticHeaders.DefaultListenerName]);

        services.AddFileHttpCommunication(configuration);

        services.AddHttpContextAccessor()
            .AddScoped<UserScopedData>();

        return services;
    }

    private static IServiceCollection AddSwaggerConfiguration(this IServiceCollection services)
    {
        services.AddSwaggerGen(c =>
        {
            c.SwaggerDoc("v1", new OpenApiInfo
            {
                Title = "My API", Version = "v1",
            });
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
                        Reference = new OpenApiReference
                        {
                            Type = ReferenceType.SecurityScheme, Id = "Bearer",
                        },
                    },
                    []
                },
            });
        });
        return services;
    }
}