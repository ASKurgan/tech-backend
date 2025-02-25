using SachkovTech.Core.Abstractions;

namespace SachkovTech.Issues.Application.Features.Lessons.Queries.GetLessons;

public record GetLessonsWithPaginationQuery(int Page, int PageSize) : IQuery;