using CSharpFunctionalExtensions;
using Microsoft.EntityFrameworkCore;
using ProjectTemplate.Application.Managers;
using ProjectTemplate.Domain;
using ProjectTemplate.Infrastructure.DbContexts;
using SharedKernel;

namespace ProjectTemplate.Infrastructure.IdentityManagers;

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