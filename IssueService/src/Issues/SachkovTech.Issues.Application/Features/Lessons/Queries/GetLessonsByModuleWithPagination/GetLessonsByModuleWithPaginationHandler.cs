using System.Text;
using CSharpFunctionalExtensions;
using Dapper;
using FileService.Communication;
using FileService.Contracts;
using FluentValidation;
using SachkovTech.Core.Abstractions;
using SachkovTech.Core.Database;
using SachkovTech.Core.Validation;
using SachkovTech.Issues.Contracts.Lesson;
using SachkovTech.Issues.Domain.ValueObjects;
using SharedKernel;

namespace SachkovTech.Issues.Application.Features.Lessons.Queries.GetLessonsByModuleWithPagination;

public class GetLessonsByModuleWithPaginationHandler
    : IQueryHandlerWithResult<PagedList<LessonDto>, GetLessonsWithPaginationQuery>
{
    private readonly IValidator<GetLessonsWithPaginationQuery> _validator;
    private readonly IFileService _fileService;
    private readonly ISqlConnectionFactory _sqlConnectionFactory;

    public GetLessonsByModuleWithPaginationHandler(
        IValidator<GetLessonsWithPaginationQuery> validator,
        IFileService fileService,
        ISqlConnectionFactory sqlConnectionFactory)
    {
        _validator = validator;
        _fileService = fileService;
        _sqlConnectionFactory = sqlConnectionFactory;
    }

    public async Task<Result<PagedList<LessonDto>, ErrorList>> Handle(
        GetLessonsWithPaginationQuery query, CancellationToken cancellationToken = default)
    {
        var validationResult = await _validator.ValidateAsync(query, cancellationToken);
        if (validationResult.IsValid == false)
            return validationResult.ToList();

        using var connection = _sqlConnectionFactory.Create();

        var parameters = new DynamicParameters();
        parameters.Add("@ModuleId", query.ModuleId);

        var sqlBuilder = new StringBuilder(
            """
            SELECT
                l.id AS Id,
                l.module_id AS ModuleId,
                l.title AS Title,
                l.description AS Description,
                l.experience AS Experience,
                l.tags AS Tags,
                l.issues AS Issues,
                l.file_id AS VideoFileId,
                l.file_location AS VideoFileLocation,
                lp.position AS Position
            FROM issues.lessons AS l
                     JOIN issues.lesson_position AS lp
                          ON l.id = lp.lesson_id
            WHERE NOT l.is_deleted AND l.module_id = @ModuleId
            """);

        if (!string.IsNullOrWhiteSpace(query.Search))
        {
            sqlBuilder.Append("\nAND l.title ILIKE @Search");
            parameters.Add("@Search", $"%{query.Search}%");
        }

        sqlBuilder.Append("\nORDER BY lp.position ASC\n");

        sqlBuilder.ApplyPagination(parameters, query.Page, query.PageSize);

        var totalCountSql = new StringBuilder(
            """
            SELECT COUNT(*)
            FROM issues.lessons AS l
            JOIN issues.lesson_position AS lp ON l.id = lp.lesson_id
            WHERE NOT l.is_deleted
            AND l.module_id = @ModuleId
            """);

        if (!string.IsNullOrWhiteSpace(query.Search))
        {
            totalCountSql.Append("\nAND l.title ILIKE @Search");
        }

        long totalCount = await connection.ExecuteScalarAsync<long>(
            totalCountSql.ToString(),
            parameters);

        var lessons = await connection.QueryAsync<LessonDto>(
            sqlBuilder.ToString(),
            param: parameters);

        var fileLocations = lessons
            .Select(l => new FileLocation(l.VideoId.ToString(), l.FileLocation))
            .ToList();

        if (fileLocations.Count == 0)
        {
            return new PagedList<LessonDto>
            {
                Items = lessons.ToList(), TotalCount = totalCount, PageSize = query.PageSize, Page = query.Page,
            };
        }

        var videoUrlsResult = await _fileService.GetDownloadUrls(new GetDownloadUrlsRequest(fileLocations), cancellationToken);
        if (videoUrlsResult.IsFailure)
            return Errors.General.NotFound().ToErrorList();

        var videoUrls = videoUrlsResult.Value.FileUrls
            .Where(f => f != null)
            .Select(f => new { f.FileId, f.Url });

        lessons = lessons.Select(l =>
        {
            var videoUrl = videoUrls.FirstOrDefault(f => f.FileId == l.VideoId.ToString())?.Url ?? string.Empty;

            return new LessonDto
            {
                Id = l.Id,
                ModuleId = l.ModuleId,
                Title = l.Title,
                Description = l.Description,
                Experience = l.Experience,
                Tags = l.Tags,
                Issues = l.Issues,
                VideoId = l.VideoId,
                FileLocation = l.FileLocation,
                PreviewId = l.PreviewId,
                Position = l.Position,
                VideoUrl = videoUrl,
            };
        });

        return new PagedList<LessonDto>
        {
            Items = lessons.ToList(), TotalCount = totalCount, PageSize = query.PageSize, Page = query.Page,
        };
    }
}