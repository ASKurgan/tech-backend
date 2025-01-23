namespace EmailNotification.Contracts.Messaging;

/// <summary>
/// Command for sending emails through EmailNotificationService.
/// </summary>
/// <param name="Email">Reciever's email address.</param>
/// <param name="Subject">Email's subject.</param>
/// <param name="Template">Template name (key).</param>
/// <param name="Data">Reciever's specific data for templating.</param>
public record SendEmailCommand(
    string Email, 
    string Subject, 
    string Template, 
    Dictionary<string, string> Data);

public record SendEmailConfirmationCommand(string Email, Dictionary<string, string> Data) 
    : SendEmailCommand(Email, "Подтвердите адрес вашей почты", "email-confirmation", Data);
