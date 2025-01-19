using EmailNotification.Contracts.Messaging;
using EmailNotificationService.API.Services;
using MassTransit;

namespace EmailNotificationService.API.Consumers;

/// <summary>
/// MassTransit+RabbitMQ consumer.
/// </summary>
public class SendEmailConsumer : IConsumer<SendEmailCommand>
{
    private readonly SendEmailService _sendEmailService;

    public SendEmailConsumer(SendEmailService sendEmailService) =>
        _sendEmailService = sendEmailService;

    /// <summary>
    /// Main MassTransit+RabbitMQ consumer for sending emails.
    /// </summary>
    /// <param name="context"></param>
    /// <returns></returns>
    /// <exception cref="ApplicationException"> Throwed if sending was unsuccsessfull.</exception>
    public async Task Consume(ConsumeContext<SendEmailCommand> context)
    {
        var sendResult = await _sendEmailService.Execute(context.Message);
        if (sendResult.IsFailure)
            throw new ApplicationException(sendResult.Error);
    }
}