using CSharpFunctionalExtensions;
using NotificationService.Entities;
using NotificationService.Entities.ValueObjects;
using SharedKernel;

namespace NotificationService.Services.Senders;

public class EmailNotificationSender : INotificationSender
{
    public Task<UnitResult<Error>> SendAsync(MessageData message, UserNotificationSettings userNotificationSetting, CancellationToken cancellationToken)
    {
        return Task.FromResult(UnitResult.Success<Error>());
    }

    public bool CanSend(UserNotificationSettings userNotificationSetting, CancellationToken cancellationToken)
    {
        return true;
    }
}