using SachkovTech.Issues.Contracts.Dtos;

namespace SachkovTech.Issues.Contracts.Module;

public record ModuleResponse(Guid Id, string Title, string Description, IssuePositionDto[] IssuesPosition, LessonPositionDto[] LessonPositions);