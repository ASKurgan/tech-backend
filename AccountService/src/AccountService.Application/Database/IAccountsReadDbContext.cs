using AccountService.Application.DataModels;
using AccountService.Contracts.Dtos;

namespace AccountService.Application.Database;

public interface IAccountsReadDbContext
{
    IQueryable<UserDataModel> Users { get; }

    IQueryable<RoleDto> Roles { get; }

    IQueryable<StudentAccountDto> StudentAccounts { get; }

    IQueryable<SupportAccountDto> SupportAccounts { get; }
}