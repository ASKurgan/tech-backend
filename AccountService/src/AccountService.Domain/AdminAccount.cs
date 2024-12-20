namespace ProjectTemplate.Domain;

public class AdminAccount
{
    public const string ADMIN = "Admin";

    //ef core
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
    private AdminAccount()
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
    {
    }

    public AdminAccount(User user)
    {
        Id = Guid.NewGuid();
        User = user;
    }

    public Guid Id { get; set; }

    public User User { get; set; }
}