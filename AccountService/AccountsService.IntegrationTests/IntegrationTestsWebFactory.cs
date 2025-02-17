using System.Data.Common;
using AccountService.Api;
using AccountService.Application.Database;
using AccountService.Infrastructure.DbContexts;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Npgsql;
using Respawn;
using SachkovTech.Core.Database;
using Testcontainers.PostgreSql;
using Testcontainers.RabbitMq;
using Testcontainers.Redis;

namespace AccountsService.IntegrationTests;

public class IntegrationTestsWebFactory : WebApplicationFactory<Program>, IAsyncLifetime
{
    private readonly PostgreSqlContainer _dbContainer = new PostgreSqlBuilder()
        .WithImage("postgres")
        .WithDatabase("sachkov_tech")
        .WithUsername("postgres")
        .WithPassword("postgres")
        .Build();

    private readonly RedisContainer _redisContainer = new RedisBuilder()
        .WithImage("redis:latest")
        .Build();

    private readonly RabbitMqContainer _rabbitMqContainer = new RabbitMqBuilder()
        .WithImage("rabbitmq:3-management")
        .WithUsername("guest")
        .WithPassword("guest")
        .Build();

    private Respawner _respawner = default!;
    private DbConnection _dbConnection = default!;

    public async Task InitializeAsync()
    {
        await _dbContainer.StartAsync();
        await _redisContainer.StartAsync();
        await _rabbitMqContainer.StartAsync();

        using var scope = Services.CreateScope();
        var issuesDbContext = scope.ServiceProvider.GetRequiredService<AccountsWriteDbContext>();

        await issuesDbContext.Database.EnsureDeletedAsync();
        await issuesDbContext.Database.EnsureCreatedAsync();

        _dbConnection = new NpgsqlConnection(_dbContainer.GetConnectionString());
        await InitializeRespawner();
    }

    public async Task ResetDatabaseAsync()
    {
        await _respawner.ResetAsync(_dbConnection);
    }

    public new async Task DisposeAsync()
    {
        await _dbContainer.StopAsync();
        await _dbContainer.DisposeAsync();

        await _redisContainer.StopAsync();
        await _redisContainer.DisposeAsync();

        await _rabbitMqContainer.StopAsync();
        await _rabbitMqContainer.DisposeAsync();
    }

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureAppConfiguration((_, config) =>
        {
            config.AddInMemoryCollection(new Dictionary<string, string?>
            {
                { "ConnectionStrings:Database", _dbContainer.GetConnectionString() },
                { "ConnectionStrings:Redis", _redisContainer.GetConnectionString() },
                { "RabbitMQ:Host", _rabbitMqContainer.GetConnectionString() },
                { "RabbitMQ:Username", "guest" },
                { "RabbitMQ:Password", "guest" },
            });
        });

        builder.ConfigureTestServices(ConfigureDefaultServices);
    }

    protected virtual void ConfigureDefaultServices(IServiceCollection services)
    {
        services.RemoveAll(typeof(AccountsWriteDbContext));
        services.RemoveAll(typeof(IAccountsReadDbContext));
        services.RemoveAll(typeof(IAutoSeeder));

        services.AddScoped<AccountsWriteDbContext>(_ =>
            new AccountsWriteDbContext(_dbContainer.GetConnectionString()));

        services.AddScoped<IAccountsReadDbContext, AccountsReadDbContext>(_ =>
            new AccountsReadDbContext(_dbContainer.GetConnectionString()));

        services.AddSingleton<IAutoSeeder, FakeAccountsSeeder>();
    }

    private async Task InitializeRespawner()
    {
        await _dbConnection.OpenAsync();
        _respawner = await Respawner.CreateAsync(
            _dbConnection,
            new RespawnerOptions { DbAdapter = DbAdapter.Postgres, SchemasToInclude = ["accounts"], });
    }
}

public class FakeAccountsSeeder : IAutoSeeder
{
    public Task SeedAsync()
    {
        return Task.CompletedTask;
    }
}