using System.Data.Common;
using Microsoft.EntityFrameworkCore.Storage;
using ProjectTemplate.Infrastructure.DbContexts;
using SachkovTech.Core.Database;

namespace ProjectTemplate.Infrastructure;

internal class UnitOfWork : IUnitOfWork
{
    private readonly AccountsWriteDbContext _accountsWriteDbContext;

    public UnitOfWork(AccountsWriteDbContext accountsWriteDbContext)
    {
        _accountsWriteDbContext = accountsWriteDbContext;
    }

    public async Task<DbTransaction> BeginTransaction(CancellationToken cancellationToken = default)
    {
        var transaction = await _accountsWriteDbContext.Database.BeginTransactionAsync(cancellationToken);

        return transaction.GetDbTransaction();
    }

    public async Task SaveChanges(CancellationToken cancellationToken = default)
    {
        await _accountsWriteDbContext.SaveChangesAsync(cancellationToken);
    }
}