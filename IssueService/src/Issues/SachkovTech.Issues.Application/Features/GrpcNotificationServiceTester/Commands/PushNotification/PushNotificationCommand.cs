
using SachkovTech.Core.Abstractions;
using SachkovTech.Issues.Contracts.Dtos;

namespace SachkovTech.Issues.Application.Features.GrpcNotificationServiceTester.Commands.PushNotification;

public record PushNotificationCommand(
    MessageDto Message,
    Guid[] UserIds,
    Guid[] RoleIds,
    string Type,
    string Data) : ICommand;