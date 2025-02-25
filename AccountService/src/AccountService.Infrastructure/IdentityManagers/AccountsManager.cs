using AccountService.Application.Managers;
using AccountService.Domain;
using AccountService.Infrastructure.DbContexts;

namespace AccountService.Infrastructure.IdentityManagers;

public class AccountsManager(AccountsDbContext accountsContext) : IAccountsManager
{
    public async Task CreateAdminAccount(AdminAccount adminAccount)
    {
        await accountsContext.AdminAccounts.AddAsync(adminAccount);
    }

    public async Task CreateParticipantAccount(
        ParticipantAccount participantAccount,
        CancellationToken cancellationToken = default)
    {
        await accountsContext.ParticipantAccounts.AddAsync(participantAccount, cancellationToken);
    }

    public async Task CreateStudentAccount(
        StudentAccount studentAccount,
        CancellationToken cancellationToken = default)
    {
        await accountsContext.StudentAccounts.AddAsync(studentAccount, cancellationToken);
    }
}