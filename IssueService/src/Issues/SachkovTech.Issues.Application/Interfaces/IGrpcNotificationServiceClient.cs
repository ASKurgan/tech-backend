using SachkovTech.Issues.Application.Features.GrpcNotificationServiceTester.Commands.AddNotificationSettings;
using SachkovTech.Issues.Application.Features.GrpcNotificationServiceTester.Commands.PushNotification;

namespace SachkovTech.Issues.Application.Interfaces;
public interface IGrpcNotificationServiceClient
{
    Task<Guid> AddNotificationSettingsAsync(AddNotificationSettingsCommand command, CancellationToken cancellationToken = default);
    Task<Guid> PushNotificationAsync(PushNotificationCommand command, CancellationToken cancellationToken = default);
}
