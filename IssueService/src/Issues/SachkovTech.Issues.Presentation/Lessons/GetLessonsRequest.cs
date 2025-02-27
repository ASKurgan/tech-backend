namespace SachkovTech.Issues.Presentation.Lessons;

public record GetLessonsRequest(int Page, int PageSize, Guid ModuleId, string? Search);