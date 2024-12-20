using CSharpFunctionalExtensions;
using Microsoft.EntityFrameworkCore;
using NotificationService.Entities;
using NotificationService.Infrastructure;

using SharedKernel;

namespace NotificationService.Features.GetNotificationSettings;

public class GetNotificationSettingsHandler
{
    private readonly NotificationSettingsDbContext _dbContext;

    public GetNotificationSettingsHandler(NotificationSettingsDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<Result<UserNotificationSettings, Error>> Handle(
        GetNotificationSettingsQuery query,
        CancellationToken cancellationToken = default)
    {
        var notificationSettings = await _dbContext.NotificationSettings
            .FirstOrDefaultAsync(x => x.Id == query.Id, cancellationToken);

        if (notificationSettings == null)
            return Error.NotFound($"No settings were found with id: {query.Id}",
                "notification.settings.not.found");

        return notificationSettings;
    }
}