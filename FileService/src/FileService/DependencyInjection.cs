using System.Reflection;
using Amazon.S3;
using FileService.BackgroundServices;
using FileService.Options;
using FileService.Services;
using MassTransit;
using MassTransit.Logging;
using MassTransit.Monitoring;
using SachkovTech.Framework.Authorization;
using SachkovTech.Framework.Endpoints;
using SachkovTech.Framework.Logging;
using SachkovTech.Framework.Observability;
using SachkovTech.Framework.Swagger;

namespace FileService;

public static class DependencyInjection
{
    public static IServiceCollection AddProgramDependencies(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddInfrastructure(configuration);

        services.AddCustomSwagger(configuration);

        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen();

        services.AddApplicationLoggingSeq(configuration);

        services.AddAuthServices(configuration);

        services.AddEndpoints(Assembly.GetExecutingAssembly());

        services.AddCors();

        services.AddBackgroundServices();

        services.AddObservability(configuration, [InstrumentationOptions.MeterName],
            [DiagnosticHeaders.DefaultListenerName]);

        return services;
    }

    private static IServiceCollection AddBackgroundServices(this IServiceCollection services)
    {
        services.AddHostedService<CancelMultipartUploadService>();

        return services;
    }
}

public static class DependencyInjectionInfrastructure
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddMessageBus(configuration)
            .AddMinio(configuration)
            .FileServices(configuration);

        return services;
    }

    private static IServiceCollection FileServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddScoped<IS3Provider, S3Provider>();

        return services;
    }

    private static IServiceCollection AddMinio(this IServiceCollection services, IConfiguration configuration) =>
        services.AddSingleton<IAmazonS3>(_ =>
        {
            var minioOptions = configuration.GetSection(MinioOptions.MINIO).Get<MinioOptions>()
                               ?? throw new ApplicationException("Missing minio configuration");

            var config = new AmazonS3Config
            {
                ServiceURL = minioOptions.Endpoint, ForcePathStyle = true, UseHttp = true,
            };

            return new AmazonS3Client(minioOptions.AccessKey, minioOptions.SecretKey, config);
        });

    private static IServiceCollection AddMessageBus(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddMassTransit(configure =>
        {
            configure.SetKebabCaseEndpointNameFormatter();

            // configure.AddConsumer<VideoProcessConsumer>();

            configure.UsingRabbitMq((context, cfg) =>
            {
                cfg.Host(new Uri(configuration["RabbitMQ:Host"]!), h =>
                {
                    h.Username(configuration["RabbitMQ:UserName"]!);
                    h.Password(configuration["RabbitMQ:Password"]!);
                });

                cfg.ConfigureEndpoints(context);
            });
        });

        return services;
    }
}