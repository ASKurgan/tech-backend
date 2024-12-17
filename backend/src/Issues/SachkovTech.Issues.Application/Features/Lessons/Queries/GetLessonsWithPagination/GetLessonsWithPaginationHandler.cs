using CSharpFunctionalExtensions;
using FileService.Communication;
using FileService.Contracts;
using FluentValidation;
using SachkovTech.Core.Abstractions;
using SachkovTech.Core.Extensions;
using SachkovTech.Core.Models;
using SachkovTech.Issues.Application.DataModels;
using SachkovTech.Issues.Application.Interfaces;
using SachkovTech.Issues.Contracts.Lesson;
using SharedKernel;

namespace SachkovTech.Issues.Application.Features.Lessons.Queries.GetLessonsWithPagination;

public class GetLessonsWithPaginationHandler(
    IValidator<GetLessonsWithPaginationQuery> validator,
    IFileService fileHttpClient,
    IReadDbContext context)
    : IQueryHandlerWithResult<PagedList<LessonResponse>, GetLessonsWithPaginationQuery>
{
    public async Task<Result<PagedList<LessonResponse>, ErrorList>> Handle(
        GetLessonsWithPaginationQuery query, CancellationToken cancellationToken = default)
    {
        var validationResult = await validator.ValidateAsync(query, cancellationToken);
        if (validationResult.IsValid == false)
            return validationResult.ToList();
        var lessonsQuery = context.Lessons;
        var lessonsPagedList = await lessonsQuery.ToPagedList(query.Page, query.PageSize, cancellationToken);

        var videoIds = lessonsPagedList.Items
            .SelectMany(l => new[] { l.VideoId, l.PreviewId })
            .ToList();
        var videoRequest = new GetFilesPresignedUrlsRequest(videoIds);

        var videoUrlsResult = await fileHttpClient.GetFilesPresignedUrls(videoRequest, cancellationToken);
        if (videoUrlsResult.IsFailure)
            return Errors.General.NotFound().ToErrorList();

        return ConvertToLessonResponses(videoUrlsResult.Value, lessonsPagedList);
    }

    private PagedList<LessonResponse> ConvertToLessonResponses(
        IReadOnlyList<FileResponse> videoUrlsResult, PagedList<LessonDataModel> lessonsPagedList)
    {
        var urls = videoUrlsResult
            .ToDictionary(v => v.FileId, u => u.PresignedUrl);
        
        var lessons = lessonsPagedList.Items
            .Select(lessonDto => new LessonResponse
            {
                Id = lessonDto.Id,
                ModuleId = lessonDto.ModuleId,
                Title = lessonDto.Title,
                Description = lessonDto.Description,
                Experience = lessonDto.Experience,
                VideoId = lessonDto.VideoId,
                VideoUrl = urls[lessonDto.VideoId],
                PreviewId = lessonDto.PreviewId,
                PreviewUrl = urls[lessonDto.PreviewId],
                //TODO: Сделать получение Tags и Issues
                Tags = [], 
                Issues = []
            }).ToList();
        
        return new PagedList<LessonResponse>
        {
            Items = lessons.AsReadOnly(),
            TotalCount = lessonsPagedList.TotalCount,
            PageSize = lessonsPagedList.PageSize,
            Page = lessonsPagedList.Page
        };
    }
}
