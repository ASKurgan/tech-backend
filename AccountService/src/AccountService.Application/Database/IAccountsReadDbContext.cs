using AccountService.Contracts.Dtos;
using ProjectTemplate.Application.DataModels;

namespace ProjectTemplate.Application.Database;

public interface IAccountsReadDbContext
{
    IQueryable<UserDataModel> Users { get; }
    
    IQueryable<RoleDto> Roles { get; }
    
    IQueryable<StudentAccountDto> StudentAccounts { get; }
    
    IQueryable<SupportAccountDto> SupportAccounts { get; }
}