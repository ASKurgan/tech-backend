using System.Reflection;
using Amazon.S3;
using FileService.Options;
using FileService.Services;
using Microsoft.OpenApi.Models;
using SachkovTech.Framework.Authorization;
using SachkovTech.Framework.Endpoints;

namespace FileService;

public static class DependencyInjection
{
    public static IServiceCollection AddProgramDependencies(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen();
        services.AddSwaggerConfiguration();

        services.AddAuthServices(configuration);

        services.AddEndpoints(Assembly.GetExecutingAssembly());

        services.AddCors();

        services
            .AddMinio(configuration)
            .FileServices(configuration);

        services.AddHttpContextAccessor()
            .AddScoped<UserScopedData>();

        // services.AddScoped<VideoProcessor>();
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

    private static IServiceCollection AddSwaggerConfiguration(this IServiceCollection services)
    {
        services.AddSwaggerGen(c =>
        {
            c.SwaggerDoc("v1", new OpenApiInfo { Title = "My API", Version = "v1", });
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