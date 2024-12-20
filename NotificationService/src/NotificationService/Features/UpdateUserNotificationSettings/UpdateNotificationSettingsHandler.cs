using CSharpFunctionalExtensions;
using Microsoft.EntityFrameworkCore;
using NotificationService.Infrastructure;

using SharedKernel;

namespace NotificationService.Features.UpdateUserNotificationSettings;

public class UpdateNotificationSettingsHandler
{
    private readonly NotificationSettingsDbContext _dbContext;
    public UpdateNotificationSettingsHandler(NotificationSettingsDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<UnitResult<Error>> Handle(
        UpdateNotificationSettingsCommand command,
        CancellationToken cancellationToken = default)
    {
        var notificationSettings = await _dbContext.NotificationSettings
            .FirstOrDefaultAsync(x => x.Id == command.Id, cancellationToken);

        if (notificationSettings == null)
            return Error.NotFound($"No settings were found with id: {command.Id}", "notification.settings.not.found");

        switch (command.NotificationType.Trim().ToLower())
        {
            case "email":
                notificationSettings.SendEmail = command.Value;
                break;
            case "telegram":
                notificationSettings.SendTelegram = command.Value;
                break;
            case "web":
                notificationSettings.SendWeb = command.Value;
                break;
        }

        await _dbContext.SaveChangesAsync(cancellationToken);

        return Result.Success<Error>();
    }
}