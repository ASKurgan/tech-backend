using AccountService.Application.Managers;
using AccountService.Domain;
using AccountService.Infrastructure.DbContexts;
using CSharpFunctionalExtensions;
using Microsoft.EntityFrameworkCore;
using SharedKernel;

namespace AccountService.Infrastructure.IdentityManagers;

public class RefreshSessionManager(AccountsWriteDbContext accountsWriteContext) : IRefreshSessionManager
{
    public async Task<Result<RefreshSession, Error>> GetByRefreshToken(
        Guid refreshToken, CancellationToken cancellationToken)
    {
        var refreshSession = await accountsWriteContext.RefreshSessions
            .Include(r => r.User)
            .ThenInclude(u => u.Roles)
            .FirstOrDefaultAsync(r => r.RefreshToken == refreshToken, cancellationToken);

        if (refreshSession is null)
            return Errors.General.NotFound(refreshToken);

        return refreshSession;
    }

    public void Delete(RefreshSession refreshSession)
    {
        accountsWriteContext.RefreshSessions.Remove(refreshSession);
    }
}