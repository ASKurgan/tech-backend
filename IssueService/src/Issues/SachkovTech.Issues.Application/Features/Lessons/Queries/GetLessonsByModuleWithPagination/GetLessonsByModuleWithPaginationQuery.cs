using SachkovTech.Core.Abstractions;

namespace SachkovTech.Issues.Application.Features.Lessons.Queries.GetLessonsByModuleWithPagination;

public record GetLessonsWithPaginationQuery(int Page, int PageSize, Guid ModuleId, string? Search) : IQuery;