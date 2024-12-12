using System.Data.Common;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Npgsql;
using Respawn;
using SachkovTech.Accounts.Infrastructure.DbContexts;
using SachkovTech.Core.Abstractions;
using SachkovTech.Issues.Application.Interfaces;
using SachkovTech.Issues.Infrastructure.DbContexts;
using Testcontainers.PostgreSql;

namespace SachkovTech.Issues.IntegrationTests;

public class IntegrationTestsWebFactory : WebApplicationFactory<Program>, IAsyncLifetime
{
    private readonly PostgreSqlContainer _dbContainer = new PostgreSqlBuilder()
        .WithImage("postgres")
        .WithDatabase("sachkov_tech")
        .WithUsername("postgres")
        .WithPassword("postgres")
        .Build();

    private Respawner _respawner = default!;
    private DbConnection _dbConnection = default!;

    public async Task InitializeAsync()
    {
        await _dbContainer.StartAsync();

        using var scope = Services.CreateScope();
        var issuesDbContext = scope.ServiceProvider.GetRequiredService<IssuesWriteDbContext>();

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
    }

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureTestServices(ConfigureDefaultServices);
    }

    protected virtual void ConfigureDefaultServices(IServiceCollection services)
    {
        services.RemoveAll(typeof(IssuesWriteDbContext));
        services.RemoveAll(typeof(IReadDbContext));
        services.RemoveAll(typeof(AccountsWriteDbContext));
        services.RemoveAll(typeof(IAutoSeeder));

        services.AddScoped<IssuesWriteDbContext>(_ =>
            new IssuesWriteDbContext(_dbContainer.GetConnectionString()));

        services.AddScoped<IReadDbContext, IssuesReadDbContext>(_ =>
            new IssuesReadDbContext(_dbContainer.GetConnectionString()));

        services.AddScoped<AccountsWriteDbContext>(_ =>
            new AccountsWriteDbContext(_dbContainer.GetConnectionString()));

        services.AddSingleton<IAutoSeeder, FakeAccountsSeeder>();
    }

    private async Task InitializeRespawner()
    {
        await _dbConnection.OpenAsync();
        _respawner = await Respawner.CreateAsync(_dbConnection, new RespawnerOptions
            {
                DbAdapter = DbAdapter.Postgres, SchemasToInclude = ["issues"],
            }
        );
    }
}

public class FakeAccountsSeeder : IAutoSeeder
{
    public Task SeedAsync()
    {
        return Task.CompletedTask;
    }
}