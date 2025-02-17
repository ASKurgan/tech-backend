using AccountService.Domain;
using CSharpFunctionalExtensions;
using SharedKernel;

namespace AccountService.Application.Database;

public interface IUserRepository
{
    Task<Result<User, Error>> GetById(Guid userId, CancellationToken cancellationToken = default);

    Task<User?> GetByPhoneNumber(
        string phoneNumber,
        CancellationToken cancellationToken = default);
}