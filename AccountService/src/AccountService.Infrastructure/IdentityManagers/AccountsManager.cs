using AccountService.Application.Managers;
using AccountService.Domain;
using AccountService.Infrastructure.DbContexts;

namespace AccountService.Infrastructure.IdentityManagers;

public class AccountsManager(AccountsWriteDbContext accountsWriteContext) : IAccountsManager
{
    public async Task CreateAdminAccount(AdminAccount adminAccount)
    {
        await accountsWriteContext.AdminAccounts.AddAsync(adminAccount);
    }

    public async Task CreateParticipantAccount(
        ParticipantAccount participantAccount,
        CancellationToken cancellationToken = default)
    {
        await accountsWriteContext.ParticipantAccounts.AddAsync(participantAccount, cancellationToken);
    }

    public async Task CreateStudentAccount(
        StudentAccount studentAccount,
        CancellationToken cancellationToken = default)
    {
        await accountsWriteContext.StudentAccounts.AddAsync(studentAccount, cancellationToken);
    }
}