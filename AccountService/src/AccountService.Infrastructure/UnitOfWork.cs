using System.Data.Common;
using AccountService.Infrastructure.DbContexts;
using Microsoft.EntityFrameworkCore.Storage;
using SachkovTech.Core.Database;

namespace AccountService.Infrastructure;

internal class UnitOfWork : IUnitOfWork
{
    private readonly AccountsDbContext _accountsDbContext;

    public UnitOfWork(AccountsDbContext accountsDbContext)
    {
        _accountsDbContext = accountsDbContext;
    }

    public async Task<DbTransaction> BeginTransaction(CancellationToken cancellationToken = default)
    {
        var transaction = await _accountsDbContext.Database.BeginTransactionAsync(cancellationToken);

        return transaction.GetDbTransaction();
    }

    public async Task SaveChanges(CancellationToken cancellationToken = default)
    {
        await _accountsDbContext.SaveChangesAsync(cancellationToken);
    }
}