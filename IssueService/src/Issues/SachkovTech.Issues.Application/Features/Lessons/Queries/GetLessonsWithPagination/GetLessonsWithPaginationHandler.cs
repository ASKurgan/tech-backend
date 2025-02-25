using CSharpFunctionalExtensions;
using FileService.Communication;
using FileService.Contracts;
using FluentValidation;
using SachkovTech.Core.Abstractions;
using SachkovTech.Core.Database;
using SachkovTech.Core.Validation;
using SachkovTech.Issues.Application.DataModels;
using SachkovTech.Issues.Application.Interfaces;
using SachkovTech.Issues.Contracts.Lesson;
using SachkovTech.Issues.Domain.Lesson;
using SharedKernel;

namespace SachkovTech.Issues.Application.Features.Lessons.Queries.GetLessonsWithPagination;

public class GetLessonsWithPaginationHandler
    : IQueryHandlerWithResult<PagedList<LessonResponse>, GetLessonsWithPaginationQuery>
{
    private readonly IValidator<GetLessonsWithPaginationQuery> _validator;
    private readonly IFileService _fileService;
    private readonly IIssuesReadDbContext _readDbContext;

    public GetLessonsWithPaginationHandler(
        IValidator<GetLessonsWithPaginationQuery> validator,
        IFileService fileService,
        IIssuesReadDbContext readDbContext)
    {
        _validator = validator;
        _fileService = fileService;
        _readDbContext = readDbContext;
    }

    public async Task<Result<PagedList<LessonResponse>, ErrorList>> Handle(
        GetLessonsWithPaginationQuery query, CancellationToken cancellationToken = default)
    {
        var validationResult = await _validator.ValidateAsync(query, cancellationToken);
        if (validationResult.IsValid == false)
            return validationResult.ToList();

        var lessonsQuery = _readDbContext.ReadLessons;
        var lessonsPagedList = await lessonsQuery.ToPagedList(query.Page, query.PageSize, cancellationToken);

        var fileLocations = lessonsPagedList.Items
            .Select(l => new FileLocation(l.Video.FileId.ToString(), l.Video.FileLocation));

        var urlsRequest = new GetDownloadUrlsRequest(fileLocations);

        var videoUrlsResult = await _fileService.GetDownloadUrls(urlsRequest, cancellationToken);
        if (videoUrlsResult.IsFailure)
            return Errors.General.NotFound().ToErrorList();

        var urlsDict = videoUrlsResult.Value.FileUrls
            .Where(f => f != null)
            .ToDictionary(f => f!.FileId, f => f!.Url);

        return ConvertToLessonResponses(urlsDict, lessonsPagedList);
    }

    private PagedList<LessonResponse> ConvertToLessonResponses(
        Dictionary<string, string> urls, PagedList<Lesson> lessonsPagedList)
    {
        var lessons = lessonsPagedList.Items
            .Select(lesson => new LessonResponse
            {
                Id = lesson.Id,
                ModuleId = lesson.ModuleId,
                Title = lesson.Title.Value,
                Description = lesson.Description.Value,
                Experience = lesson.Experience.Value,
                VideoId = lesson.Video.FileId,
                VideoUrl = urls[lesson.Video.FileId.ToString()],

                // TODO: Сделать получение превью
                PreviewId = Guid.Empty,
                PreviewUrl = string.Empty,

                // TODO: Сделать получение Tags и Issues
                Tags = [],
                Issues = [],
            }).ToList();

        return new PagedList<LessonResponse>
        {
            Items = lessons.AsReadOnly(), TotalCount = lessonsPagedList.TotalCount, PageSize = lessonsPagedList.PageSize, Page = lessonsPagedList.Page,
        };
    }
}