using Amazon.S3;
using FileService.Extensions;
using FileService.Options;
using FileService.Services;

namespace FileService;

public static class DependencyInjection
{
    public static IServiceCollection AddProgramDependencies(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen();

        services.AddEndpoints();

        services.AddCors();

        services
            .AddMinio(configuration)
            .FileServices(configuration);

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
}