using AccountService.Application.Database;
using AccountService.Contracts.Responses;
using CSharpFunctionalExtensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using SachkovTech.Core.Abstractions;
using SachkovTech.Core.Caching;
using SharedKernel;

namespace AccountService.Application.Queries.GetUserById;

public class GetUserByIdHandler : IQueryHandlerWithResult<UserResponse, GetUserByIdQuery>
{
    private readonly DistributedCacheEntryOptions _cacheOptions = new()
    {
        SlidingExpiration = TimeSpan.FromMinutes(5),
    };

    private readonly IAccountsReadDbContext _accountsReadDbContext;
    private readonly ICacheService _cache;

    public GetUserByIdHandler(IAccountsReadDbContext accountsReadDbContext, ICacheService cache)
    {
        _accountsReadDbContext = accountsReadDbContext;
        _cache = cache;
    }

    public async Task<Result<UserResponse, ErrorList>> Handle(
        GetUserByIdQuery query,
        CancellationToken cancellationToken = default)
    {
        var key = "users_" + query.UserId;

        var userResponse = await _cache.GetOrSetAsync(
            key,
            _cacheOptions,
            async () => await GetUserById(query, cancellationToken),
            cancellationToken);

        if (userResponse is null)
            return Errors.General.NotFound(query.UserId).ToErrorList();

        return userResponse;
    }

    private async Task<UserResponse?> GetUserById(GetUserByIdQuery query, CancellationToken cancellationToken)
    {
        var user = await _accountsReadDbContext.Users
            .Include(u => u.StudentAccount)
            .Include(u => u.SupportAccount)
            .Include(u => u.AdminAccount)
            .Include(u => u.Roles)
            .FirstOrDefaultAsync(u => u.Id == query.UserId, cancellationToken);

        if (user is null)
            return null;

        return new UserResponse(
            user.Id,
            user.UserName,
            user.FirstName,
            user.SecondName,
            user.ThirdName,
            user.Email,
            user.PhoneNumber,
            user.RegistrationDate,
            user.SocialNetworks,
            user.StudentAccount,
            user.SupportAccount,
            user.AdminAccount,
            user.Roles);
    }
}