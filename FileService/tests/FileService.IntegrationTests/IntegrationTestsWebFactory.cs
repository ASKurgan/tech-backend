using Amazon.S3;
using FileService.IntegrationTests.Auth;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using SachkovTech.Core.Caching;
using Testcontainers.Minio;
using Testcontainers.Redis;


namespace FileService.IntegrationTests;

public class IntegrationTestsWebFactory : WebApplicationFactory<Program>, IAsyncLifetime
{
    private readonly MinioContainer _minioContainer = new MinioBuilder()
        .WithImage("minio/minio")
        .WithUsername("minioadmin")
        .WithPassword("minioadmin")
        .Build();

    private readonly RedisContainer _redisContainer = new RedisBuilder()
        .WithImage("redis")
        .Build();

    public async Task InitializeAsync()
    {
        await _minioContainer.StartAsync();
        await _redisContainer.StartAsync();
    }

    public new async Task DisposeAsync()
    {
        await _minioContainer.StopAsync();
        await _minioContainer.DisposeAsync();

        await _redisContainer.StopAsync();
        await _redisContainer.DisposeAsync();
    }

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureTestServices(services =>
        {
            services.RemoveAll(sd => sd.ServiceType == typeof(AuthenticationHandler<>));
            services.RemoveAll<IAuthorizationPolicyProvider>();
            services.RemoveAll<IAuthorizationHandler>();

            services.AddSingleton<IAuthorizationPolicyProvider, TestPolicyProvider>();
            services.AddSingleton<IAuthorizationHandler, TestRequirementHandler>();

            services.AddAuthentication(options =>
                {
                    options.DefaultAuthenticateScheme = "Test";
                    options.DefaultChallengeScheme = "Test";
                    options.DefaultSignInScheme = "Test";
                })
                .AddScheme<AuthenticationSchemeOptions, TestAuthHandler>("Test", null);

            services.RemoveAll<IAmazonS3>();

            ushort port = _minioContainer.GetMappedPublicPort(9000);

            services.AddSingleton<IAmazonS3>(_ =>
            {
                var config = new AmazonS3Config
                {
                    ServiceURL = $"http://{_minioContainer.Hostname}:{port}", UseHttp = true, ForcePathStyle = true,
                };

                return new AmazonS3Client("minioadmin", "minioadmin", config);
            });

            services.RemoveAll<ICacheService>();
            services.RemoveAll<IDistributedCache>();

            ushort redisPort = _redisContainer.GetMappedPublicPort(6379);
            string redisConnectionString = $"{_redisContainer.Hostname}:{redisPort}";

            var configuration = new ConfigurationBuilder()
                .AddInMemoryCollection(new Dictionary<string, string?>
                {
                    { "ConnectionStrings:Redis", redisConnectionString }, { "Minio:ExpirationTimeInDays", "7" },
                })
                .Build();

            services.AddDistributedCache(configuration);
        });
    }
}