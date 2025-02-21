namespace FaqService.Contracts;

public record CreatePostRequest(
    string Title,
    string Description,
    string ReplLink,
    Guid UserId,
    Guid? IssueId,
    Guid? LessonId,
    List<Guid> Tags);