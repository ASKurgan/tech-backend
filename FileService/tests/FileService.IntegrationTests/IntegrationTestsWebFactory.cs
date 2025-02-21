using Amazon.S3;
using FileService.IntegrationTests.Auth;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Testcontainers.Minio;

namespace FileService.IntegrationTests;

public class IntegrationTestsWebFactory : WebApplicationFactory<Program>, IAsyncLifetime
{
    private readonly MinioContainer _minioContainer = new MinioBuilder()
        .WithImage("minio/minio")
        .WithUsername("minioadmin")
        .WithPassword("minioadmin")
        .Build();

    public async Task InitializeAsync()
    {
        await _minioContainer.StartAsync();
    }

    public new async Task DisposeAsync()
    {
        await _minioContainer.StopAsync();
        await _minioContainer.DisposeAsync();
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
        });
    }
}