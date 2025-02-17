using AccountService.Domain;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Logging;

namespace AccountService.Infrastructure.DbContexts;

public class AccountsWriteDbContext : IdentityDbContext<User, Role, Guid>
{
    private readonly string _connectionString;

    public AccountsWriteDbContext(string connectionString)
    {
        _connectionString = connectionString;
    }

    public DbSet<RolePermission> RolePermissions => Set<RolePermission>();

    public DbSet<Permission> Permissions => Set<Permission>();

    public DbSet<AdminAccount> AdminAccounts => Set<AdminAccount>();

    public DbSet<ParticipantAccount> ParticipantAccounts => Set<ParticipantAccount>();

    public DbSet<StudentAccount> StudentAccounts => Set<StudentAccount>();

    public DbSet<RefreshSession> RefreshSessions => Set<RefreshSession>();

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseNpgsql(_connectionString);
        optionsBuilder.UseSnakeCaseNamingConvention();
        optionsBuilder.EnableSensitiveDataLogging();
        optionsBuilder.UseLoggerFactory(CreateLoggerFactory());
        optionsBuilder.ConfigureWarnings(warnings =>
            warnings.Ignore(RelationalEventId.PendingModelChangesWarning));
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Role>()
            .ToTable("roles");

        modelBuilder.Entity<IdentityUserClaim<Guid>>()
            .ToTable("user_claims");

        modelBuilder.Entity<IdentityUserToken<Guid>>()
            .ToTable("user_tokens");

        modelBuilder.Entity<IdentityUserLogin<Guid>>()
            .ToTable("user_logins");

        modelBuilder.Entity<IdentityRoleClaim<Guid>>()
            .ToTable("role_claims");

        modelBuilder.Entity<IdentityUserRole<Guid>>()
            .ToTable("user_roles");

        modelBuilder.ApplyConfigurationsFromAssembly(
            typeof(AccountsWriteDbContext).Assembly,
            type => type.FullName?.Contains("Configurations.Write") ?? false);

        modelBuilder.HasDefaultSchema("accounts");
    }

    private ILoggerFactory CreateLoggerFactory() =>
        LoggerFactory.Create(builder => { builder.AddConsole(); });
}