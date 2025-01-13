namespace SachkovTech.Issues.Contracts.Lesson;

public record UpdateLessonPositionRequest(
    Guid ModuleId,
    int Position);