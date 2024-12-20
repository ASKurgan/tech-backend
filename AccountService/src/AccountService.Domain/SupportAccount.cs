namespace ProjectTemplate.Domain;

public class SupportAccount
{
    public const string SUPPORT = "Support";

    //EF CORE
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
    private SupportAccount()
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
    {
        
    }

    public SupportAccount(
        User user, 
        string aboutSelf)
    {
        Id = Guid.NewGuid();
        User = user;
        AboutSelf = aboutSelf;
    }
    
    public Guid Id { get; set; }
    
    public Guid UserId { get; set; }
    
    public User User { get; set; }
    
    public string AboutSelf { get; set; }
}