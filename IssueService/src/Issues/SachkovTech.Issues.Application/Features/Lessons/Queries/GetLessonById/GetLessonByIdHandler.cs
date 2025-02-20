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

public class GetLessonByIdHandler : IQueryHandlerWithResult<LessonResponse, GetLessonByIdQuery>
{
    private readonly IReadDbContext _context;
    private readonly IFileService _fileService;

    public GetLessonByIdHandler(IReadDbContext context, IFileService fileService)
    {
        _context = context;
        _fileService = fileService;
    }

    public async Task<Result<LessonResponse, ErrorList>> Handle(
        GetLessonByIdQuery query, CancellationToken cancellationToken = default)
    {
        var lesson = await _context.Lessons.FirstOrDefaultAsync(l => l.Id == query.LessonId, cancellationToken);
        if (lesson is null)
            return Errors.General.NotFound(query.LessonId, "lesson").ToErrorList();

        var fileServiceRequest = new GetDownloadUrlRequest(lesson.VideoId.ToString(), lesson.FileLocation);

        var urlResult = await _fileService.GetDownloadUrl(fileServiceRequest, cancellationToken);
        if (urlResult.IsFailure)
            return Errors.General.NotFound().ToErrorList();

        var lessonResponse = ToLessonResponse(lesson, urlResult.Value.DownloadUrl);

        return lessonResponse;
    }

    private LessonResponse ToLessonResponse(LessonDataModel lesson, string url) =>
        new()
        {
            Id = lesson.Id,
            ModuleId = lesson.ModuleId,
            Title = lesson.Title,
            Description = lesson.Description,
            Experience = lesson.Experience,
            VideoId = lesson.VideoId,
            VideoUrl = url,
            PreviewId = lesson.PreviewId,

            // TODO: Сделайть получение превью
            PreviewUrl = string.Empty,

            // TODO: Сделать получение Tags и Issues
            Tags = [],
            Issues = [],
        };
}