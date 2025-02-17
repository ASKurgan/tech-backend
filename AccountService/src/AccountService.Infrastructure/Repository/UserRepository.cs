using AccountService.Application.Database;
using AccountService.Domain;
using AccountService.Infrastructure.DbContexts;
using CSharpFunctionalExtensions;
using Microsoft.EntityFrameworkCore;
using SharedKernel;

namespace AccountService.Infrastructure.Repository;

public class UserRepository : IUserRepository
{
    private readonly AccountsWriteDbContext _dbContext;

    public UserRepository(AccountsWriteDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<Result<User, Error>> GetById(Guid userId, CancellationToken cancellationToken = default)
    {
        var user = await _dbContext.Users.FirstOrDefaultAsync(u => u.Id == userId, cancellationToken);
        if (user is null)
            return Errors.General.NotFound(userId, nameof(userId));

        return user;
    }

    public async Task<User?> GetByPhoneNumber(
        string phoneNumber,
        CancellationToken cancellationToken = default)
    {
        var user = await _dbContext.Users.FirstOrDefaultAsync(p => p.PhoneNumber == phoneNumber, cancellationToken);

        return user;
    }
}