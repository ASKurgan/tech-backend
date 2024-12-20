namespace ProjectTemplate.Domain;

public class ParticipantAccount
{
    public const string PARTICIPANT = "Participant";

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
    private ParticipantAccount()
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
    {
        
    }

    public ParticipantAccount(User user)
    {
        Id = Guid.NewGuid();
        User = user;
    }
    
    public Guid Id { get; set; }
    
    public Guid UserId { get; set; }
    
    public User User { get; set; }
}