using CSharpFunctionalExtensions;
using FileService.Communication;
using FileService.Contracts;
using Microsoft.EntityFrameworkCore;
using SachkovTech.Core.Abstractions;
using SachkovTech.Issues.Application.DataModels;
using SachkovTech.Issues.Application.Interfaces;
using SachkovTech.Issues.Contracts.Lesson;
using SachkovTech.Issues.Domain.Lesson;
using SharedKernel;

namespace SachkovTech.Issues.Application.Features.Lessons.Queries.GetLessonById;

public class GetLessonByIdHandler : IQueryHandlerWithResult<LessonResponse, GetLessonByIdQuery>
{
    private readonly IIssuesReadDbContext _readDbContext;
    private readonly IFileService _fileService;

    public GetLessonByIdHandler(IIssuesReadDbContext readDbContext, IFileService fileService)
    {
        _readDbContext = readDbContext;
        _fileService = fileService;
    }

    public async Task<Result<LessonResponse, ErrorList>> Handle(
        GetLessonByIdQuery query, CancellationToken cancellationToken = default)
    {
        var lesson = await _readDbContext.ReadLessons.FirstOrDefaultAsync(l => l.Id == query.LessonId, cancellationToken);
        if (lesson is null)
            return Errors.General.NotFound(query.LessonId, "lesson").ToErrorList();

        var fileServiceRequest = new GetDownloadUrlRequest(lesson.Video.FileId.ToString(), lesson.Video.FileLocation);

        var urlResult = await _fileService.GetDownloadUrl(fileServiceRequest, cancellationToken);
        if (urlResult.IsFailure)
            return Errors.General.NotFound().ToErrorList();

        var lessonResponse = ToLessonResponse(lesson, urlResult.Value.DownloadUrl);

        return lessonResponse;
    }

    private LessonResponse ToLessonResponse(Lesson lesson, string url) =>
        new()
        {
            Id = lesson.Id,
            ModuleId = lesson.ModuleId,
            Title = lesson.Title.Value,
            Description = lesson.Description.Value,
            Experience = lesson.Experience.Value,
            VideoId = lesson.Video.FileId,
            VideoUrl = url,

            // TODO: Сделайть получение превью
            PreviewId = Guid.Empty,
            PreviewUrl = string.Empty,

            // TODO: Сделать получение Tags и Issues
            Tags = [],
            Issues = [],
        };
}