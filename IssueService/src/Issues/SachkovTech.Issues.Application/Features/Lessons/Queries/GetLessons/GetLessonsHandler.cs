using CSharpFunctionalExtensions;
using FileService.Communication;
using FileService.Contracts;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using SachkovTech.Core.Abstractions;
using SachkovTech.Core.Database;
using SachkovTech.Core.Validation;
using SachkovTech.Issues.Application.Interfaces;
using SachkovTech.Issues.Application.Mappers;
using SachkovTech.Issues.Contracts.Lesson;
using SachkovTech.Issues.Domain.ValueObjects;
using SharedKernel;

namespace SachkovTech.Issues.Application.Features.Lessons.Queries.GetLessons;

public class GetLessonsHandler : IQueryHandlerWithResult<PagedList<LessonDto>, GetLessonsWithPaginationQuery>
{
    private readonly IValidator<GetLessonsWithPaginationQuery> _validator;
    private readonly IFileService _fileService;
    private readonly IIssuesReadDbContext _readDbContext;

    public GetLessonsHandler(
        IValidator<GetLessonsWithPaginationQuery> validator,
        IFileService fileService,
        IIssuesReadDbContext readDbContext)
    {
        _validator = validator;
        _fileService = fileService;
        _readDbContext = readDbContext;
    }

    public async Task<Result<PagedList<LessonDto>, ErrorList>> Handle(
        GetLessonsWithPaginationQuery query, CancellationToken cancellationToken = default)
    {
        var validationResult = await _validator.ValidateAsync(query, cancellationToken);
        if (validationResult.IsValid == false)
            return validationResult.ToList();

        var module = await _readDbContext.ReadModules.FirstOrDefaultAsync(m => m.Id == query.ModuleId, cancellationToken);
        if (module is null)
        {
            return new PagedList<LessonDto>
            {
                Items = [], TotalCount = 0, PageSize = query.PageSize, Page = query.Page,
            };
        }

        var lessonsQuery = _readDbContext.ReadLessons;

        int totalCount = await lessonsQuery.CountAsync(cancellationToken);

        if (!string.IsNullOrWhiteSpace(query.Search))
        {
            lessonsQuery = lessonsQuery.Where(l => EF.Functions.Like(l.Title.Value.ToLower(), $"%{query.Search.ToLower()}%"));
        }

        var items = await lessonsQuery
            .Where(l => l.ModuleId == query.ModuleId)
            .Skip((query.Page - 1) * query.PageSize)
            .Take(query.PageSize)
            .ToListAsync(cancellationToken: cancellationToken);

        var positionsDict = module.LessonsPosition
            .ToDictionary(l => l.LessonId, l => l.Position);

        var fileLocations = items
            .Select(l => new FileLocation(l.Video.FileId.ToString(), l.Video.FileLocation))
            .ToList();

        if (fileLocations.Count == 0)
        {
            return new PagedList<LessonDto>
            {
                Items = items.Select(l => l.ToDto(positionsDict, null)).ToList(), TotalCount = totalCount, PageSize = query.PageSize, Page = query.Page,
            };
        }

        var videoUrlsRequest = new GetDownloadUrlsRequest(fileLocations);

        var videoUrlsResult = await _fileService.GetDownloadUrls(videoUrlsRequest, cancellationToken);
        if (videoUrlsResult.IsFailure)
            return Errors.General.NotFound().ToErrorList();

        var videoUrls = videoUrlsResult.Value.FileUrls
            .Where(f => f != null)
            .ToDictionary(f => new Video(Guid.Parse(f!.FileId)), f => f!.Url);

        return new PagedList<LessonDto>
        {
            Items = items.Select(l => l.ToDto(positionsDict, videoUrls)).ToList(), TotalCount = totalCount, PageSize = query.PageSize, Page = query.Page,
        };
    }
}