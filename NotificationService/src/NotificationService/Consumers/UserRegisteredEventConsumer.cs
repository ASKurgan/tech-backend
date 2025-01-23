using AccountService.Communication;
using AccountService.Contracts.Messaging;
using EmailNotification.Contracts.Messaging;
using MassTransit;

namespace NotificationService.Consumers;

public class UserRegisteredEventConsumer : IConsumer<UserRegisteredEvent>
{
    private readonly IAccountService _accountService;

    public UserRegisteredEventConsumer(IAccountService accountService)
    {
        _accountService = accountService;
    }

    public async Task Consume(ConsumeContext<UserRegisteredEvent> context)
    {
        if (context.Message.UserId == Guid.Empty)
            return;

        var result = await _accountService.GetConfirmationLink(context.Message.UserId);

        if (result.IsFailure)
            throw new ApplicationException(result.Error);

        var sendEmailCommand = new SendEmailConfirmationCommand(
            result.Value.Email,
             new Dictionary<string, string>()
            {
                { "FullName", "result.Value.UserName" },
                { "ConfirmationLink", result.Value.ConfirmationLink }
            });

        await context.Publish(sendEmailCommand, context.CancellationToken);
    }
}