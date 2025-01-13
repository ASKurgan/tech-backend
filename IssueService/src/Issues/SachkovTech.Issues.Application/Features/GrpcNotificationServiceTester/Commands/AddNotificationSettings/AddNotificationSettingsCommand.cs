using SachkovTech.Core.Abstractions;

namespace SachkovTech.Issues.Application.Features.GrpcNotificationServiceTester.Commands.AddNotificationSettings;

public record AddNotificationSettingsCommand(
    Guid UserId,
    string Email,
    string? WebEndpoint) : ICommand;