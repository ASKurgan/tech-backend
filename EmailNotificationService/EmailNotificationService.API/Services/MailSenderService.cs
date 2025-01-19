using Microsoft.Extensions.Options;
using MimeKit;
using MailKit.Net.Smtp;
using CSharpFunctionalExtensions;
using EmailNotificationService.API.Models;
using EmailNotificationService.API.Options;

namespace EmailNotificationService.API.Services;

/// <summary>
/// Service for sending emails via smtp server.
/// </summary>
public class MailSenderService
{
    private readonly MailOptions _options;
    private readonly ILogger<MailSenderService> _logger;
    private readonly EmailValidator _validator;

    public MailSenderService(
        IOptions<MailOptions> options,
        ILogger<MailSenderService> logger,
        EmailValidator validator)
    {
        _options = options.Value;
        _logger = logger;
        _validator = validator;
    }

    /// <summary>
    /// This method creates MimeKit's message, which represents email with all its data, 
    /// then opens connection with SMTP-server and sends email.
    /// </summary>
    /// <param name="mailData">DTO with email content and recepients.</param>
    /// <returns></string></returns>
    public async Task<UnitResult<string>> Send(MailData mailData)
    {
        var validationResult = _validator.Execute(mailData.To);
        if (validationResult.IsFailure)
            return validationResult.Error;

        mailData.To = validationResult.Value;

        var mail = new MimeMessage();

        mail.From.Add(new MailboxAddress(_options.FromDisplayName, _options.From));

        foreach (var address in mailData.To)
        {
            MailboxAddress.TryParse(address, out var mailAddress);
            mail.To.Add(mailAddress!);
        }

        var body = new BodyBuilder { HtmlBody = mailData.Body };

        mail.Body = body.ToMessageBody();
        mail.Subject = mailData.Subject;

        using var client = new SmtpClient();

        await client.ConnectAsync(_options.Host, _options.Port);
        await client.AuthenticateAsync(_options.UserName, _options.Password);
        await client.SendAsync(mail);

        foreach (var address in mail.To)
            _logger.LogInformation("Email succesfully sended to {to}", address);

        return UnitResult.Success<string>();
    }
}