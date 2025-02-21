namespace FaqService.Contracts;

public record UpdatePostRefAndTagsRequest(
    string ReplLink,
    Guid? IssueId,
    Guid? LessonId,
    List<Guid> Tags);