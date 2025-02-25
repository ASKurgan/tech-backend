using AccountService.Application.Database;
using AccountService.Domain;
using AccountService.Infrastructure.DbContexts;
using CSharpFunctionalExtensions;
using Microsoft.EntityFrameworkCore;
using SharedKernel;
using Errors = SharedKernel.Errors;

namespace AccountService.Infrastructure.Repository;

public class UserRepository : IUserRepository
{
    private readonly AccountsDbContext _dbContext;

    public UserRepository(AccountsDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<Result<User, Error>> GetById(Guid userId, CancellationToken cancellationToken = default)
    {
        var user = await _dbContext.ReadUsers.FirstOrDefaultAsync(u => u.Id == userId, cancellationToken);
        if (user is null)
            return Errors.General.NotFound(userId, nameof(userId));

        return user;
    }

    public async Task<User?> GetByPhoneNumber(
        string phoneNumber,
        CancellationToken cancellationToken = default)
    {
        var user = await _dbContext.ReadUsers.FirstOrDefaultAsync(p => p.PhoneNumber == phoneNumber, cancellationToken);

        return user;
    }

    public async Task<bool> IsUserExistsByUserName(string userName, CancellationToken cancellationToken)
    {
        var user = await _dbContext.ReadUsers.FirstOrDefaultAsync(p => p.UserName == userName, cancellationToken);

        if (user is null)
            return false;

        return true;
    }
}