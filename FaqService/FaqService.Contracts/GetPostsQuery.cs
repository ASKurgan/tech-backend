using FaqService.Contracts.Enums;

namespace FaqService.Contracts;

public record GetPostsQuery(
    string? SearchText,
    Status? Status,
    bool? SortByDateDescending,
    Guid[] Tags,
    Guid? IssueId,
    Guid? LessonId,
    Guid? Cursor,
    int Limit = 10);