using AccountService.Contracts.Dtos;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using ProjectTemplate.Application.Database;
using ProjectTemplate.Application.DataModels;

namespace ProjectTemplate.Infrastructure.DbContexts;

public class AccountsReadDbContext(IConfiguration configuration) : DbContext, IAccountsReadDbContext
{
    public IQueryable<UserDataModel> Users => Set<UserDataModel>();
    public IQueryable<RoleDto> Roles => Set<RoleDto>();
    public IQueryable<StudentAccountDto> StudentAccounts => Set<StudentAccountDto>();
    public IQueryable<SupportAccountDto> SupportAccounts => Set<SupportAccountDto>();

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseNpgsql(configuration.GetConnectionString("Database"));
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