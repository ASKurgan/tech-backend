using CSharpFunctionalExtensions;
using FileService.Communication;
using FileService.Contracts;
using Microsoft.EntityFrameworkCore;
using SachkovTech.Core.Abstractions;
using SachkovTech.Issues.Application.DataModels;
using SachkovTech.Issues.Application.Interfaces;
using SachkovTech.Issues.Contracts.Lesson;
using SharedKernel;

namespace SachkovTech.Issues.Application.Features.Lessons.Queries.GetLessonById;

public class GetLessonByIdHandler(
    IReadDbContext context,
    IFileService fileService) : IQueryHandlerWithResult<LessonResponse, GetLessonByIdQuery>
{
    public async Task<Result<LessonResponse, ErrorList>> Handle(
        GetLessonByIdQuery query, CancellationToken cancellationToken = default)
    {
        var lesson = await context.Lessons.FirstOrDefaultAsync(l => l.Id == query.LessonId, cancellationToken);
        if (lesson is null)
            return Errors.General.NotFound(query.LessonId, "lesson").ToErrorList();

        var fileServiceRequest = new GetFilesPresignedUrlsRequest([lesson.VideoId, lesson.PreviewId]);

        var urlsResult = await fileService.GetFilesPresignedUrls(fileServiceRequest, cancellationToken);
        if (urlsResult.IsFailure)
            return Errors.General.NotFound().ToErrorList();

        var urls = urlsResult.Value.ToDictionary(v => v.FileId, u => u.PresignedUrl);

        var lessonResponse = ToLessonResponse(lesson, urls);

        return lessonResponse;
    }

    private LessonResponse ToLessonResponse(LessonDataModel lesson, Dictionary<Guid, string> urls) =>
        new LessonResponse
        {
            Id = lesson.Id,
            ModuleId = lesson.ModuleId,
            Title = lesson.Title,
            Description = lesson.Description,
            Experience = lesson.Experience,
            VideoId = lesson.VideoId,
            VideoUrl = urls[lesson.VideoId],
            PreviewId = lesson.PreviewId,
            PreviewUrl = urls[lesson.PreviewId],
            //TODO: Сделать получение Tags и Issues
            Tags = [],
            Issues = []
        };
}