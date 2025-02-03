namespace ProjectTemplate.Domain;

public class StudentAccount
{
    public const string STUDENT = "Student";

    private StudentAccount()
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