using SachkovTech.Issues.Contracts.Issue;

namespace SachkovTech.Issues.Contracts.Lesson;

public class LessonDto
{
    public Guid Id { get; init; }

    public Guid ModuleId { get; init; }

    public int Position { get; init; }

    public string Title { get; init; } = string.Empty;

    public string Description { get; init; } = string.Empty;

    public int Experience { get; init; }

    public Guid VideoId { get; init; }

    public string FileLocation { get; init; } = string.Empty;

    public string VideoUrl { get; init; } = string.Empty;

    public Guid PreviewId { get; init; }

    public string PreviewUrl { get; init; } = string.Empty;

    public Guid[] Tags { get; init; } = [];

    public Guid[] Issues { get; init; } = [];
}