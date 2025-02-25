using AccountService.Domain;

namespace AccountService.Application.Database;

public interface IAccountsReadDbContext
{
    IQueryable<User> ReadUsers { get; }

    IQueryable<Role> ReadRoles { get; }

    IQueryable<StudentAccount> ReadStudentAccounts { get; }

    IQueryable<SupportAccount> ReadSupportAccounts { get; }
}