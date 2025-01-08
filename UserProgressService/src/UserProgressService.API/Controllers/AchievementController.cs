using Microsoft.AspNetCore.Mvc;
using SachkovTech.Framework;
using UserProgressService.API.Controllers.Requests;
using UserProgressService.Application.Features.Achievements.Commands.Create.ForIssue;
using UserProgressService.Application.Features.Achievements.Commands.Create.ForLesson;
using UserProgressService.Application.Features.Achievements.Commands.Delete;
using UserProgressService.Application.Features.Achievements.Commands.Update.ForIssue;
using UserProgressService.Application.Features.Achievements.Commands.Update.ForLesson;
using UserProgressService.Application.Features.Achievements.Commands.Update.MainInfo;
using UserProgressService.Application.Features.Achievements.Queries.GetAchievementById;

namespace UserProgressService.API.Controllers;

public class AchievementController : ApplicationController
{
    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(
        [FromRoute] Guid id,
        [FromServices] GetAchievementByIdHandler handler,
        CancellationToken cancellationToken = default)
    {
        var query = new GetAchievementByIdQuery(id);

        var result = await handler.Handle(query, cancellationToken);
        if (result.IsFailure)
            return result.Error.ToResponse();

        return Ok(result.Value);
    }

    [HttpPost("lesson")]
    public async Task<IActionResult> CreateForLesson(
        [FromBody] CreateAchievementForLessonRequest request,
        [FromServices] CreateLessonAchievementHandler handler,
        CancellationToken cancellationToken = default)
    {
        var command = new CreateLessonAchievementCommand(
            request.IconId,
            request.Name,
            request.Description,
            request.LessonCondition,
            request.Experience);

        var result = await handler.Handle(command, cancellationToken);
        if (result.IsFailure)
            return result.Error.ToResponse();

        return Ok(result.Value);
    }

    [HttpPost("issue")]
    public async Task<IActionResult> CreateForIssue(
        [FromBody] CreateIssueAchievementRequest request,
        [FromServices] CreateIssueAchievementHandler handler,
        CancellationToken cancellationToken = default)
    {
        var command = new CreateIssueAchievementCommand(
            request.IconId,
            request.Name,
            request.Description,
            request.IssueCondition,
            request.Experience);

        var result = await handler.Handle(command, cancellationToken);
        if (result.IsFailure)
            return result.Error.ToResponse();

        return Ok(result.Value);
    }

    [HttpPut("{id:guid}/main-info")]
    public async Task<IActionResult> UpdateMainInfo(
        [FromRoute] Guid id,
        [FromBody] UpdateMainInfoAchievementRequest request,
        [FromServices] UpdateMainInfoAchievementHandler handler,
        CancellationToken cancellationToken = default)
    {
        var command = new UpdateMainInfoAchievementCommand(
            id,
            request.IconId,
            request.Name,
            request.Description,
            request.Experience);

        var result = await handler.Handle(command, cancellationToken);
        if (result.IsFailure)
            return result.Error.ToResponse();

        return Ok(result.Value);
    }

    [HttpPut("{id:guid}/condition-for-lesson")]
    public async Task<IActionResult> UpdateConditionForLesson(
        [FromRoute] Guid id,
        [FromBody] UpdateLessonAchievementConditionRequest request,
        [FromServices] UpdateLessonAchievementConditionHandler handler,
        CancellationToken cancellationToken = default)
    {
        var command = new UpdateLessonAchievementConditionCommand(
            id,
            request.IssueCondition);

        var result = await handler.Handle(command, cancellationToken);
        if (result.IsFailure)
            return result.Error.ToResponse();

        return Ok(result.Value);
    }

    [HttpPut("{id:guid}/condition-for-issue")]
    public async Task<IActionResult> UpdateConditionForIssue(
        [FromRoute] Guid id,
        [FromBody] UpdateIssueAchievementConditionRequest request,
        [FromServices] UpdateIssueAchievementConditionHandler handler,
        CancellationToken cancellationToken = default)
    {
        var command = new UpdateIssueAchievementConditionCommand(
            id,
            request.IssueCondition);

        var result = await handler.Handle(command, cancellationToken);
        if (result.IsFailure)
            return result.Error.ToResponse();

        return Ok(result.Value);
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(
        [FromRoute] Guid id,
        [FromServices] DeleteAchievementHandler handler,
        CancellationToken cancellationToken = default)
    {
        var command = new DeleteAchievementCommand(id);

        var result = await handler.Handle(command, cancellationToken);
        if (result.IsFailure)
            return result.Error.ToResponse();

        return Ok(result.Value);
    }
}