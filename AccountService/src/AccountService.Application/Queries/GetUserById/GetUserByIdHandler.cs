using AccountService.Application.Database;
using AccountService.Application.Mappers;
using AccountService.Contracts.Responses;
using CSharpFunctionalExtensions;
using Microsoft.EntityFrameworkCore;
using SachkovTech.Core.Abstractions;
using SharedKernel;

namespace AccountService.Application.Queries.GetUserById;

public class GetUserByIdHandler : IQueryHandlerWithResult<UserDto, GetUserByIdQuery>
{
    private readonly IAccountsReadDbContext _accountsReadDbContext;

    public GetUserByIdHandler(IAccountsReadDbContext accountsReadDbContext)
    {
        _accountsReadDbContext = accountsReadDbContext;
    }

    public async Task<Result<UserDto, ErrorList>> Handle(
        GetUserByIdQuery query,
        CancellationToken cancellationToken = default)
    {
        var userResponse = await GetUserById(query, cancellationToken);

        if (userResponse is null)
            return Errors.General.NotFound(query.UserId).ToErrorList();

        return userResponse;
    }

    private async Task<UserDto?> GetUserById(GetUserByIdQuery query, CancellationToken cancellationToken)
    {
        var user = await _accountsReadDbContext.ReadUsers
            .Include(u => u.StudentAccount)
            .Include(u => u.SupportAccount)
            .Include(u => u.AdminAccount)
            .Include(u => u.Roles)
            .FirstOrDefaultAsync(u => u.Id == query.UserId, cancellationToken);

        return user?.ToUserDto();
    }
}