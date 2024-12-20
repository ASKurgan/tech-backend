using SachkovTech.Core.Abstractions;

namespace SachkovTech.Issues.Application.Features.Issue.Queries.GetIssuesByModuleWithPagination;

public record GetFilteredIssuesByModuleWithPaginationQuery(
    Guid ModuleId,
    string? Title,
    string? SortBy,
    string? SortDirection,
    int Page,
    int PageSize) : IQuery;