using SachkovTech.Issues.Contracts.Dtos;

namespace SachkovTech.Issues.Contracts.Module;

public record ModuleDto(
    Guid Id,
    string Title,
    string Description,
    IReadOnlyList<IssuePositionDto> IssuesPositions,
    IReadOnlyList<LessonPositionDto> LessonPositions);