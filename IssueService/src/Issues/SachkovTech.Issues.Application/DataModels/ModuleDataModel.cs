using SachkovTech.Issues.Contracts.Dtos;

namespace SachkovTech.Issues.Application.DataModels;

public class ModuleDataModel
{
    public Guid Id { get; init; }

    public string Title { get; init; } = string.Empty;

    public string Description { get; init; } = string.Empty;

    public IssuePositionDto[] IssuesPosition { get; init; } = [];
    public LessonPositionDto[] LessonsPosition { get; init; } = [];

    public bool IsDeleted { get; init; }
}