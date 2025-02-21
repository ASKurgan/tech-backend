using FaqService.Contracts.Enums;

namespace FaqService.Dtos;

public record PostDto
{
    public Guid Id { get; init; }
    public string Title { get; init; }
    public string Description { get; init; }
    public string ReplLink { get; init; }
    public Guid UserId { get; init; }
    public Guid? IssueId { get; init; }
    public Guid? LessonId { get; init; }
    public List<Guid>? Tags { get; init; }
    public Status Status { get; init; }
    public Guid? AnswerId { get; init; }
    public DateTime CreatedAt { get; init; }
    public int CountOfAnswers { get; init; }
}