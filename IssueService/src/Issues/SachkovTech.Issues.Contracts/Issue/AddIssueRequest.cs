namespace SachkovTech.Issues.Contracts.Issue;

public record AddIssueRequest(
    Guid ModuleId,
    Guid? LessonId,
    string Title,
    string Description,
    int Experience);