namespace SachkovTech.Issues.Contracts.IssueSolving;

public record GetUserIssuesByModuleWithPaginationRequest(
    Guid UserId,
    Guid ModuleId,
    string? Status,
    int Page,
    int PageSize);