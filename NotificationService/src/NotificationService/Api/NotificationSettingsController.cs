using Microsoft.AspNetCore.Mvc;
using NotificationService.Api.Contracts;
using NotificationService.Features.GetNotificationSettings;
using NotificationService.Features.UpdateUserNotificationSettings;
using SachkovTech.Framework;
using SharedKernel;

namespace NotificationService.Api;

[Route("[controller]/{id:Guid}")]
[ApiController]
public class NotificationSettingsController : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> Get(
        [FromRoute] Guid id,
        [FromServices] GetNotificationSettingsHandler handler,
        CancellationToken cancellationToken = default)
    {
        var query = new GetNotificationSettingsQuery(id);

        var result = await handler.Handle(query, cancellationToken);
        if (result.IsFailure)
            return result.Error.ToResponse();

        var envelope = Envelope.Ok(result.Value);
        return Ok(envelope);
    }

    [HttpPatch]
    public async Task<IActionResult> Patch(
        [FromRoute] Guid id,
        [FromBody] PatchNotificationSettingsRequest dto,
        [FromServices] UpdateNotificationSettingsHandler handler,
        CancellationToken cancellationToken = default)
    {
        var command = new UpdateNotificationSettingsCommand(
            id, dto.NotificationType, dto.Value);

        var result = await handler.Handle(command, cancellationToken);
        if (result.IsFailure)
            return result.Error.ToResponse();

        return Ok();
    }
}