namespace ProjectTemplate.Domain;

public class StudentAccount
{
    public const string STUDENT = "Student";

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
    private StudentAccount()
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
    {
        
    }

    public StudentAccount(User user)
    {
        Id = Guid.NewGuid();
        User = user;
        DateStartedStudying = DateTime.UtcNow;
    }
    
    public Guid Id { get; set; }
    
    public Guid UserId { get; set; }
    
    public User User { get; set; }
    
    public DateTime DateStartedStudying { get; set; }
}