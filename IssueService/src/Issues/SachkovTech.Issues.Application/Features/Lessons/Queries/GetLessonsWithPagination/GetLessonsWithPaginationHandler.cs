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
using SharedKernel;

namespace SachkovTech.Issues.Application.Features.Lessons.Queries.GetLessonsWithPagination;

public class GetLessonsWithPaginationHandler
    : IQueryHandlerWithResult<PagedList<LessonResponse>, GetLessonsWithPaginationQuery>
{
    private readonly IValidator<GetLessonsWithPaginationQuery> _validator;
    private readonly IFileService _fileService;
    private readonly IReadDbContext _context;

    public GetLessonsWithPaginationHandler(
        IValidator<GetLessonsWithPaginationQuery> validator,
        IFileService fileService,
        IReadDbContext context)
    {
        _validator = validator;
        _fileService = fileService;
        _context = context;
    }

    public async Task<Result<PagedList<LessonResponse>, ErrorList>> Handle(
        GetLessonsWithPaginationQuery query, CancellationToken cancellationToken = default)
    {
        var validationResult = await _validator.ValidateAsync(query, cancellationToken);
        if (validationResult.IsValid == false)
            return validationResult.ToList();

        var lessonsQuery = _context.Lessons;
        var lessonsPagedList = await lessonsQuery.ToPagedList(query.Page, query.PageSize, cancellationToken);

        var fileLocations = lessonsPagedList.Items
            .Select(l => new FileLocation(l.FileId.ToString(), l.FileLocation));

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
        Dictionary<string, string> urls, PagedList<LessonDataModel> lessonsPagedList)
    {
        var lessons = lessonsPagedList.Items
            .Select(lessonDto => new LessonResponse
            {
                Id = lessonDto.Id,
                ModuleId = lessonDto.ModuleId,
                Title = lessonDto.Title,
                Description = lessonDto.Description,
                Experience = lessonDto.Experience,
                VideoId = lessonDto.FileId,
                VideoUrl = urls[lessonDto.FileId.ToString()],
                PreviewId = lessonDto.PreviewId,

                // TODO: Сделать получение превью
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