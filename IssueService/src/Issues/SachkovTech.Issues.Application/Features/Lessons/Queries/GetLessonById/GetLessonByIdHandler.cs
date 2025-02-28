using CSharpFunctionalExtensions;
using FileService.Communication;
using FileService.Contracts;
using Microsoft.EntityFrameworkCore;
using SachkovTech.Core.Abstractions;
using SachkovTech.Core.Database;
using SachkovTech.Issues.Application.Interfaces;
using SachkovTech.Issues.Application.Mappers;
using SachkovTech.Issues.Contracts.Lesson;
using SachkovTech.Issues.Domain.Lesson;
using SachkovTech.Issues.Domain.ValueObjects;
using SharedKernel;

namespace SachkovTech.Issues.Application.Features.Lessons.Queries.GetLessonById;

public class GetLessonByIdHandler : IQueryHandlerWithResult<LessonDto, GetLessonByIdQuery>
{
    private readonly IIssuesReadDbContext _readDbContext;
    private readonly IFileService _fileService;

    public GetLessonByIdHandler(IIssuesReadDbContext readDbContext, IFileService fileService)
    {
        _readDbContext = readDbContext;
        _fileService = fileService;
    }

    public async Task<Result<LessonDto, ErrorList>> Handle(
        GetLessonByIdQuery query, CancellationToken cancellationToken = default)
    {
        // TODO: доделать получение позиции в модуле
        var lesson = await _readDbContext.ReadLessons.FirstOrDefaultAsync(l => l.Id == query.LessonId, cancellationToken);
        if (lesson is null)
            return Errors.General.NotFound(query.LessonId, "lesson").ToErrorList();

        var fileLocation = new FileLocation(lesson.Video.FileId.ToString(), lesson.Video.FileLocation);

        var videoUrlsRequest = new GetDownloadUrlsRequest([fileLocation]);

        var videoUrlsResult = await _fileService.GetDownloadUrls(videoUrlsRequest, cancellationToken);
        if (videoUrlsResult.IsFailure)
            return Errors.General.NotFound().ToErrorList();

        var videoUrls = videoUrlsResult.Value.FileUrls
            .Where(f => f != null)
            .ToDictionary(f => new Video(Guid.Parse(f!.FileId)), f => f!.Url);

        return lesson.ToDto(null, videoUrls);
    }
}