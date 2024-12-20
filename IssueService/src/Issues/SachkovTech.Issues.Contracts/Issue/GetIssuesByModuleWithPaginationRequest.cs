namespace SachkovTech.Issues.Contracts.Issue;

public record GetIssuesByModuleWithPaginationRequest(
    string? Title,
    string? SortBy,
    string? SortDirection,
    int Page,
    int PageSize);