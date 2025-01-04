using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using ProjectTemplate.Application.DataModels;
using SachkovTech.Core.Database;

namespace ProjectTemplate.Application.Queries;

internal class UserQueryBuilder
{
    private IQueryable<UserDataModel> _userQuery;
    
    public UserQueryBuilder(IQueryable<UserDataModel> userQuery)
    {
        _userQuery = userQuery;
    }
    
    /// <summary>
    /// Get users with the specified id
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    public UserQueryBuilder WithId(Guid? id)
    {
        _userQuery = _userQuery.WhereIf(id is not null && id != Guid.Empty,
            ud => ud.Id == id!.Value);
        
        return this;
    }

    /// <summary>
    /// Get users with the specified role
    /// </summary>
    /// <param name="roleName"></param>
    /// <returns></returns>
    public UserQueryBuilder WithRole(string? roleName)
    {
        _userQuery = _userQuery.WhereIf(string.IsNullOrWhiteSpace(roleName) == false,
            ud => ud.Roles.Any(r => r.Name.ToLower() == roleName!.ToLower()));
        
        return this;
    }
    
    /// <summary>
    /// Get users with more than one role
    /// </summary>
    /// <returns></returns>
    public UserQueryBuilder WithMoreThanOneRole()
    {
        _userQuery = _userQuery.Where(ud => ud.Roles.Count > 1);
        
        return this;
    }
    
    /// <summary>
    /// Get users excluding specific role
    /// </summary>
    /// <param name="roleName"></param>
    /// <returns></returns>
    public UserQueryBuilder ExcludeRole(string? roleName)
    {
        _userQuery = _userQuery.WhereIf(string.IsNullOrWhiteSpace(roleName) == false, 
            ud => ud.Roles.All(r => r.Name.ToLower() != roleName!.ToLower()));
        
        return this;
    }
    
    /// <summary>
    /// Get users with the specified first name
    /// </summary>
    /// <param name="firstName"></param>
    /// <returns></returns>
    public UserQueryBuilder WithFirstName(string? firstName)
    {
        _userQuery = _userQuery.WhereIf(string.IsNullOrWhiteSpace(firstName) == false,
            ud => ud.FirstName == firstName);
        
        return this;
    }
    
    /// <summary>
    /// Get users with the first name which starts with the specified sequence
    /// </summary>
    /// <param name="sequence"></param>
    /// <returns></returns>
    public UserQueryBuilder WithFirstNameStartingWith(string? sequence)
    {
        _userQuery = _userQuery.WhereIf(string.IsNullOrWhiteSpace(sequence) == false,
            ud => ud.FirstName.StartsWith(sequence!));
        
        return this;
    }
    
    /// <summary>
    /// Get users with the specified second name
    /// </summary>
    /// <param name="secondName"></param>
    /// <returns></returns>
    public UserQueryBuilder WithSecondName(string? secondName)
    {
        _userQuery = _userQuery.WhereIf(string.IsNullOrWhiteSpace(secondName) == false, 
            ud => ud.SecondName == secondName);
        
        return this;
    }
    
    /// <summary>
    /// Get users with the specified third name
    /// </summary>
    /// <param name="thirdName"></param>
    /// <returns></returns>
    public UserQueryBuilder WithThirdName(string? thirdName)
    {
        _userQuery = _userQuery.WhereIf(string.IsNullOrWhiteSpace(thirdName) == false, 
            ud => ud.ThirdName == thirdName);
        
        return this;
    }
    
    /// <summary>
    /// Get users with the specified email
    /// </summary>
    /// <param name="email"></param>
    /// <returns></returns>
    public UserQueryBuilder WithEmail(string? email)
    {
        _userQuery = _userQuery.WhereIf(string.IsNullOrWhiteSpace(email) == false,
            ud => ud.Email == email);
        
        return this;
    }
    
    /// <summary>
    /// Get users that were registered after the specified date
    /// </summary>
    /// <param name="registrationDate">Date to compare with</param>
    /// <returns></returns>
    public UserQueryBuilder WithRegistrationAfter(DateTime? registrationDate)
    {
        _userQuery = _userQuery.WhereIf(registrationDate is not null && registrationDate != DateTime.MinValue,
            ud => ud.RegistrationDate.Date < registrationDate!.Value.Date);
        
        return this;
    }
    
    /// <summary>
    /// Get users that have been on the platform longer than the specified number of days
    /// </summary>
    /// <param name="daysNumber">Number of days</param>
    /// <returns></returns>
    public UserQueryBuilder OnPlatformLongerThan(int daysNumber)
    {
        _userQuery = _userQuery.Where(ud => DateTime.UtcNow.Day - ud.RegistrationDate.Day > daysNumber);
        
        return this;
    }
    
    /// <summary>
    /// Sort users by the specified field
    /// </summary>
    /// <param name="sortBy"></param>
    /// <returns></returns>
    public UserQueryBuilder SortAscendingBy(string? sortBy)
    {
        _userQuery = _userQuery.OrderBy(KeySelector(sortBy));
        
        return this;
    }
    
    /// <summary>
    /// Sort users by the specified field in descending order
    /// </summary>
    /// <param name="sortBy"></param>
    /// <returns></returns>
    public UserQueryBuilder SortDescendingBy(string? sortBy)
    {
        _userQuery = _userQuery.OrderByDescending(KeySelector(sortBy));
        
        return this;
    }
    
    /// <summary>
    /// Sort users by the specified field and direction
    /// </summary>
    /// <param name="sortBy">Filter field</param>
    /// <param name="sortDirection">Filter direction</param>
    /// <returns></returns>
    public UserQueryBuilder SortByWithDirection(string? sortBy, string? sortDirection)
    {
        _userQuery = sortDirection?.ToLower() == "desc"
            ? _userQuery.OrderByDescending(KeySelector(sortBy))
            : _userQuery.OrderBy(KeySelector(sortBy));
        
        return this;
    }

    /// <summary>
    /// Include student account
    /// </summary>
    /// <returns></returns>
    public UserQueryBuilder IncludeStudentAccount()
    {
        _userQuery = _userQuery.Include(u => u.StudentAccount);

        return this;
    }
    
    /// <summary>
    /// Include support account
    /// </summary>
    /// <returns></returns>
    public UserQueryBuilder IncludeSupportAccount()
    {
        _userQuery = _userQuery.Include(u => u.SupportAccount);

        return this;
    }
    
    /// <summary>
    /// Include admin account
    /// </summary>
    /// <returns></returns>
    public UserQueryBuilder IncludeAdminAccount()
    {
        _userQuery = _userQuery.Include(u => u.AdminAccount);

        return this;
    }
    
    /// <summary>
    /// Include roles
    /// </summary>
    /// <returns></returns>
    public UserQueryBuilder IncludeRoles()
    {
        _userQuery = _userQuery.Include(u => u.Roles);

        return this;
    }

    /// <summary>
    /// Builds the query
    /// </summary>
    /// <returns></returns>
    public IQueryable<UserDataModel> Build()
    {
        return _userQuery;
    }

    private Expression<Func<UserDataModel, object>> KeySelector(string? sortBy)
    {
        return sortBy?.ToLower() switch
        {
            "email" => (user) => user.Email,
            "first_name" => (user) => user.FirstName,
            "second_name" => (user) => user.SecondName,
            "third_name" => (user) => user.ThirdName,
            _ => (user) => user.Id
       
    }

}