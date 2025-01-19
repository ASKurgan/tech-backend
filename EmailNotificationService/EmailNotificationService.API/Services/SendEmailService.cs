using CSharpFunctionalExtensions;
using EmailNotificationService.API.Models;
using EmailNotification.Contracts.Messaging;

namespace EmailNotificationService.API.Services;

/// <summary>
/// Service which handles main actions sequence: templating email body and sending email.
/// </summary>
public class SendEmailService
{
    private readonly MailSenderService _mailSender;
    private readonly HandlebarsTemplateService _handlebarTemplateService;

    public SendEmailService(
        MailSenderService mailSender, HandlebarsTemplateService handlebarTemplateService)
    {
        _mailSender = mailSender;
        _handlebarTemplateService = handlebarTemplateService;
    }

    /// <summary>
    /// Executes main actions sequence: templating email body and sending it.
    /// </summary>
    /// <param name="command">Command which was recieved through message broker.</param>
    /// <returns></returns>
    public async Task<UnitResult<string>> Execute(SendEmailCommand command)
    {
        var mailBody = _handlebarTemplateService.Process(command.Data, command.Template);

        var mailData = new MailData([command.Email], command.Subject, mailBody);

        var sendResult = await _mailSender.Send(mailData);
        if (sendResult.IsFailure)
            return sendResult.Error;

        return UnitResult.Success<string>();
    }
}