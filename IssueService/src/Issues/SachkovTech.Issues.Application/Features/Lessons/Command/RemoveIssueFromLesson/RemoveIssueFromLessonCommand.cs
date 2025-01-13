using SachkovTech.Core.Abstractions;

namespace SachkovTech.Issues.Application.Features.Lessons.Command.RemoveIssueFromLesson;

public record RemoveIssueFromLessonCommand(Guid LessonId, Guid IssueId) : ICommand;