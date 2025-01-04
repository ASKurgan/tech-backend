using EmailNotification.Contracts;
using MassTransit;

namespace EmailNotificationService.API.Consumers;

public class SendEmailConsumer : IConsumer<SendEmailCommand>
{
    public Task Consume(ConsumeContext<SendEmailCommand> context)
    {
        // send-email

        return Task.CompletedTask;
    }
}