

using SachkovTech.Core.Abstractions;

namespace SachkovTech.Issues.Application.Features.Lessons.Command.AddIssueToLesson;

public record AddIssueToLessonCommand(Guid LessonId, Guid IssueId) : ICommand;
