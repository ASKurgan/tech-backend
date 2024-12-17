using Microsoft.AspNetCore.Mvc;
using SachkovTech.Framework;
using SachkovTech.Framework.Authorization;
using SachkovTech.Issues.Application.Features.Lessons.Command.AddIssueToLesson;
using SachkovTech.Issues.Application.Features.Lessons.Command.AddLesson;
using SachkovTech.Issues.Application.Features.Lessons.Command.AddTagToLesson;
using SachkovTech.Issues.Application.Features.Lessons.Command.RemoveIssueFromLesson;
using SachkovTech.Issues.Application.Features.Lessons.Command.RestoreLesson;
using SachkovTech.Issues.Application.Features.Lessons.Command.SoftDeleteLesson;
using SachkovTech.Issues.Application.Features.Lessons.Command.StartUploadVideo;
using SachkovTech.Issues.Application.Features.Lessons.Command.UpdateLesson;
using SachkovTech.Issues.Application.Features.Lessons.Queries.GetLessonById;
using SachkovTech.Issues.Application.Features.Lessons.Queries.GetLessonsWithPagination;
using SachkovTech.Issues.Contracts.Lesson;

namespace SachkovTech.Issues.Presentation.Lessons;

public class LessonsController : ApplicationController
{
    [HttpGet]
    [Permission(Permissions.Lessons.READ_LESSON)]
    public async Task<IActionResult> GetLessonsWithPagination(
        [FromQuery] int page,
        [FromQuery] int pageSize,
        [FromServices] GetLessonsWithPaginationHandler handler,
        CancellationToken cancellationToken)
    {
        var result = await handler.Handle(new GetLessonsWithPaginationQuery(page, pageSize),
            cancellationToken);

        if (result.IsFailure)
            return result.Error.ToResponse();

        return Ok(result.Value);
    }

    [HttpGet("{lessonId:guid}")]
    [Permission(Permissions.Lessons.READ_LESSON)]
    public async Task<IActionResult> GetLessonById(
        [FromRoute] Guid lessonId,
        [FromServices] GetLessonByIdHandler handler,
        CancellationToken cancellationToken)
    {
        var result = await handler.Handle(new GetLessonByIdQuery(lessonId), cancellationToken);

        if (result.IsFailure)
            return result.Error.ToResponse();

        return Ok(result.Value);
    }

    [HttpPost("{lessonId:guid}/start-upload-video")]
    [Permission(Permissions.Lessons.UPDATE_LESSON)]
    public async Task<IActionResult> StartUploadVideo(
        [FromServices] StartUploadVideoHandler handler,
        [FromBody] FileMetadataRequest request,
        CancellationToken cancellationToken = default)
    {
        var command = new StartUploadVideoCommand(
            request.FileName,
            request.ContentType,
            request.FileSize);
    
        var result = await handler.Handle(command, cancellationToken);
    
        if (result.IsFailure)
            result.Error.ToResponse();
    
        return Ok(result.Value);
    }

    [HttpPost]
    [Permission(Permissions.Lessons.CREATE_LESSON)]
    public async Task<IActionResult> CreateLesson(
        [FromBody] AddLessonRequest request,
        [FromServices] AddLessonHandler handler,
        CancellationToken cancellationToken)
    {
        var command = new AddLessonCommand(
            request.ModuleId,
            request.Title,
            request.Description,
            request.Experience,
            request.VideoId,
            request.PreviewId,
            request.Tags,
            request.Issues,
            request.FileName,
            request.ContentType,
            request.FileSize,
            request.UploadId,
            request.Parts);

        var result = await handler.Handle(command, cancellationToken);

        if (result.IsFailure)
            return result.Error.ToResponse();

        return Ok(result.Value);
    }

    [HttpPut]
    [Permission(Permissions.Lessons.UPDATE_LESSON)]
    public async Task<IActionResult> UpdateLesson(
        [FromBody] UpdateLessonRequest request,
        [FromServices] UpdateLessonHandler handler,
        CancellationToken cancellationToken)
    {
        var command = new UpdateLessonCommand(
            request.LessonId,
            request.Title,
            request.Description,
            request.Experience,
            request.VideoId,
            request.PreviewId,
            request.Tags,
            request.Issues);

        var result = await handler.Handle(command, cancellationToken);

        if (result.IsFailure)
            return result.Error.ToResponse();

        return Ok();
    }

    [HttpPatch("{lessonId}/tag")]
    [Permission(Permissions.Lessons.UPDATE_LESSON)]
    public async Task<IActionResult> AddTagToLesson(
        [FromRoute] Guid lessonId,
        [FromBody] Guid tagId,
        [FromServices] AddTagToLessonHandler handler,
        CancellationToken cancellationToken)
    {
        var result = await handler.Handle(new AddTagToLessonCommand(lessonId, tagId), cancellationToken);

        if (result.IsFailure)
            return result.Error.ToResponse();

        return Ok();
    }

    [HttpPatch("{lessonId}/issue")]
    [Permission(Permissions.Lessons.UPDATE_LESSON)]
    public async Task<IActionResult> AddIssueToLesson(
        [FromRoute] Guid lessonId,
        [FromBody] Guid issueId,
        [FromServices] AddIssueToLessonHandler handler,
        CancellationToken cancellationToken)
    {
        var result = await handler.Handle(new AddIssueToLessonCommand(lessonId, issueId), cancellationToken);

        if (result.IsFailure)
            return result.Error.ToResponse();

        return Ok();
    }

    [HttpPatch("{lessonId}/restore")]
    [Permission(Permissions.Lessons.UPDATE_LESSON)]
    public async Task<IActionResult> RestoreLesson(
        [FromRoute] Guid lessonId,
        [FromServices] RestoreLessonHandler handler,
        CancellationToken cancellationToken)
    {
        var result = await handler.Handle(new RestoreLessonCommand(lessonId), cancellationToken);

        if (result.IsFailure)
            return result.Error.ToResponse();

        return Ok();
    }

    [HttpDelete("{lessonId}/issue")]
    [Permission(Permissions.Lessons.UPDATE_LESSON)]
    public async Task<IActionResult> RemoveIssueFromLesson(
        [FromRoute] Guid lessonId,
        [FromBody] Guid issueId,
        [FromServices] RemoveIssueFromLessonHandler handler,
        CancellationToken cancellationToken)
    {
        var result = await handler.Handle(new RemoveIssueFromLessonCommand(lessonId, issueId), cancellationToken);

        if (result.IsFailure)
            return result.Error.ToResponse();

        return Ok();
    }

    [HttpDelete("{lessonId}/tag")]
    [Permission(Permissions.Lessons.UPDATE_LESSON)]
    public async Task<IActionResult> RemoveTagFromLesson(
        [FromRoute] Guid lessonId,
        [FromBody] Guid tagId,
        [FromServices] RemoveIssueFromLessonHandler handler,
        CancellationToken cancellationToken)
    {
        var result = await handler.Handle(new RemoveIssueFromLessonCommand(lessonId, tagId), cancellationToken);

        if (result.IsFailure)
            return result.Error.ToResponse();

        return Ok();
    }

    [HttpDelete("{lessonId:guid}")]
    [Permission(Permissions.Lessons.DELETE_LESSON)]
    public async Task<IActionResult> SoftDeleteLesson(
        [FromRoute] Guid lessonId,
        [FromServices] SoftDeleteLessonHandler handler,
        CancellationToken cancellationToken)
    {
        var result = await handler.Handle(new SoftDeleteLessonCommand(lessonId), cancellationToken);

        if (result.IsFailure)
            return result.Error.ToResponse();

        return Ok();
    }
}