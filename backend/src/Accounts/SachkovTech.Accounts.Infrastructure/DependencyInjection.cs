using MassTransit;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SachkovTech.Accounts.Application.Database;
using SachkovTech.Accounts.Application.Managers;
using SachkovTech.Accounts.Application.MessageBus;
using SachkovTech.Accounts.Application.Providers;
using SachkovTech.Accounts.Domain;
using SachkovTech.Accounts.Infrastructure.DbContexts;
using SachkovTech.Accounts.Infrastructure.IdentityManagers;
using SachkovTech.Accounts.Infrastructure.Migrator;
using SachkovTech.Accounts.Infrastructure.Options;
using SachkovTech.Accounts.Infrastructure.Providers;
using SachkovTech.Accounts.Infrastructure.Seeding;
using SachkovTech.Core.Abstractions;
using SachkovTech.Core.Options;
using SachkovTech.SharedKernel;

namespace SachkovTech.Accounts.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddAccountsInfrastructure(
        this IServiceCollection services, IConfiguration configuration)
    {
        services.AddIdentity()
            .AddDbContexts(configuration)
            .AddSeeding()
            .ConfigureCustomOptions(configuration)
            .AddMessageBus(configuration)
            .AddProviders()
            .AddMigrators();

        return services;
    }

    private static IServiceCollection AddMessageBus(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddMassTransit<IAccountMessageBus>(configure =>
        {
            configure.SetKebabCaseEndpointNameFormatter();

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

    private static IServiceCollection AddMigrators(this IServiceCollection services)
    {
        services.AddScoped<IMigrator, AccountsMigrator>();

        return services;
    }

    private static IServiceCollection AddIdentity(this IServiceCollection services)
    {
        services
            .AddIdentity<User, Role>(options =>
            {
                options.User.RequireUniqueEmail = true;
            })
            .AddEntityFrameworkStores<AccountsWriteDbContext>()
            .AddDefaultTokenProviders();

        services.AddScoped<PermissionManager>();
        services.AddScoped<RolePermissionManager>();
        services.AddScoped<IAccountsManager, AccountsManager>();
        services.AddScoped<IPermissionManager, PermissionManager>();
        services.AddScoped<AccountsManager>();
        services.AddScoped<IRefreshSessionManager, RefreshSessionManager>();

        return services;
    }

    private static IServiceCollection AddDbContexts(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddScoped<AccountsWriteDbContext>(_ =>
            new AccountsWriteDbContext(configuration.GetConnectionString("Database")!));

        services.AddScoped<IAccountsReadDbContext, AccountsReadDbContext>();

        services.AddKeyedScoped<IUnitOfWork, UnitOfWork>(Modules.Accounts);

        return services;
    }

    private static IServiceCollection AddSeeding(this IServiceCollection services)
    {
        services.AddSingleton<IAutoSeeder, AccountsSeeder>();
        services.AddScoped<AccountsSeederService>();

        return services;
    }

    private static IServiceCollection ConfigureCustomOptions(this IServiceCollection services,
        IConfiguration configuration)
    {
        services.Configure<JwtOptions>(configuration.GetSection(JwtOptions.JWT));
        services.Configure<AdminOptions>(configuration.GetSection(AdminOptions.ADMIN));

        return services;
    }

    private static IServiceCollection AddProviders(this IServiceCollection services)
    {
        services.AddTransient<ITokenProvider, JwtTokenProvider>();

        return services;
    }
}