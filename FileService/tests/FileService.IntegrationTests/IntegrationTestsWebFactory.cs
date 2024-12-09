using Amazon.S3;
using FileService.Jobs;
using Hangfire;
using Hangfire.PostgreSql;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;
using MongoDB.Driver;
using Testcontainers.Minio;
using Testcontainers.MongoDb;
using Testcontainers.PostgreSql;

namespace FileService.IntegrationTests;

public class IntegrationTestsWebFactory : WebApplicationFactory<Program>, IAsyncLifetime
{
    private readonly MongoDbContainer _mongoContainer = new MongoDbBuilder()
        .WithImage("mongo")
        .WithUsername("mongoadmin")
        .WithPassword("mongopassword").Build();

    private readonly MinioContainer _minioContainer = new MinioBuilder()
        .WithImage("minio/minio")
        .WithUsername("minioadmin")
        .WithPassword("minioadmin")
        .Build();

    private readonly PostgreSqlContainer _postgreSqlContainer = new PostgreSqlBuilder()
        .WithImage("postgres")
        .WithDatabase("hangfire")
        .WithUsername("postgres")
        .WithPassword("postgres")
        .Build();

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureTestServices(services =>
        {
            services.RemoveAll(typeof(IAmazonS3));
            services.RemoveAll(typeof(IMongoClient));
            services.RemoveAll(typeof(HangfireContext));
            
            var port = _minioContainer.GetMappedPublicPort(9000);

            services.AddSingleton<IAmazonS3>(_ =>
            {
                var config = new AmazonS3Config
                {
                    ServiceURL = $"http://{_minioContainer.Hostname}:{port}",
                    ForcePathStyle = true,
                    UseHttp = true
                };

                return new AmazonS3Client("minioadmin", "minioadmin", config);
            });

            services.AddSingleton<IMongoClient>(new MongoClient(_mongoContainer.GetConnectionString()));

            services.AddScoped<HangfireContext>(_ =>
                new HangfireContext(_postgreSqlContainer.GetConnectionString()));
            
            services.AddHangfire(config => config
                .SetDataCompatibilityLevel(CompatibilityLevel.Version_180)
                .UseSimpleAssemblyNameTypeSerializer()
                .UseRecommendedSerializerSettings()
                .UsePostgreSqlStorage(options =>
                {
                    options.UseNpgsqlConnection(_postgreSqlContainer.GetConnectionString());
                }));
        });
    }

    public async Task InitializeAsync()
    {
        await _mongoContainer.StartAsync();
        await _minioContainer.StartAsync();
        
        await _postgreSqlContainer.StartAsync();
    }

    public async Task DisposeAsync()
    {
        await _mongoContainer.StopAsync();
        await _mongoContainer.DisposeAsync();
        
        await _minioContainer.StopAsync();
        await _minioContainer.DisposeAsync();

        await _postgreSqlContainer.StopAsync();
        await _postgreSqlContainer.DisposeAsync();
    }
}