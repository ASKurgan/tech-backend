using AccountService.Application.Database;
using AccountService.Application.DataModels;
using AccountService.Contracts.Dtos;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace AccountService.Infrastructure.DbContexts;

public class AccountsReadDbContext : DbContext, IAccountsReadDbContext
{
    private readonly string _connectionString;

    public AccountsReadDbContext(string connectionString)
    {
        _connectionString = connectionString;
    }

    public IQueryable<UserDataModel> Users => Set<UserDataModel>();

    public IQueryable<RoleDto> Roles => Set<RoleDto>();

    public IQueryable<StudentAccountDto> StudentAccounts => Set<StudentAccountDto>();

    public IQueryable<SupportAccountDto> SupportAccounts => Set<SupportAccountDto>();

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseNpgsql(_connectionString);
        optionsBuilder.UseSnakeCaseNamingConvention();
        optionsBuilder.EnableSensitiveDataLogging();
        optionsBuilder.UseLoggerFactory(CreateLoggerFactory());

        optionsBuilder.UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking);
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema("accounts");

        modelBuilder.ApplyConfigurationsFromAssembly(
            typeof(AccountsReadDbContext).Assembly,
            type => type.FullName?.Contains("Configurations.Read") ?? false);
    }

    private ILoggerFactory CreateLoggerFactory() =>
        LoggerFactory.Create(builder => { builder.AddConsole(); });
}