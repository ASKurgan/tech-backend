namespace EmailNotification.Contacts;

public record SendEmailCommand(string Email, string Subject, string Template, object Data);