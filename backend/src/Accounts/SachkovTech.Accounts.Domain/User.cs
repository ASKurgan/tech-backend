using CSharpFunctionalExtensions;
using Microsoft.AspNetCore.Identity;
using SachkovTech.SharedKernel.ValueObjects;
using SharedKernel;

namespace SachkovTech.Accounts.Domain;

public class User : IdentityUser<Guid>
{
    private List<Role> _roles = [];
    private List<SocialNetwork> _socialNetworks = [];

    // EF CORE
    private User()
    {
    }

    public DateTime RegistrationDate { get; set; }

    public FullName FullName { get; set; } = null!;

    public Photo? Photo { get; set; }

    public IReadOnlyList<Role> Roles => _roles;

    public IReadOnlyList<SocialNetwork> SocialNetworks => _socialNetworks;

    public StudentAccount? StudentAccount { get; private set; }

    public SupportAccount? SupportAccount { get; private set; }

    public AdminAccount? AdminAccount { get; private set; }

    public static Result<User, Error> CreateAdmin(
        string userName,
        string email,
        FullName fullName,
        Role role)
    {
        if (role.Name != AdminAccount.ADMIN)
            return Errors.User.InvalidRole();

        return new User
        {
            UserName = userName,
            Email = email,
            RegistrationDate = DateTime.UtcNow,
            FullName = fullName,
            _roles = [role],
            _socialNetworks = [],
        };
    }

    public static Result<User, Error> CreateParticipant(
        string userName,
        string email,
        Role role)
    {
        if (role.Name != ParticipantAccount.PARTICIPANT)
            return Errors.User.InvalidRole();

        return new User
        {
            UserName = userName,
            Email = email,
            RegistrationDate = DateTime.UtcNow,
            FullName = FullName.Empty,
            _roles = [role],
            _socialNetworks = [],
        };
    }

    public void EnrollParticipant(Role role)
    {
        if (!_roles.Contains(role) && role.Name == StudentAccount.STUDENT)
            _roles.Add(role);
    }
}