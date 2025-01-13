using SachkovTech.Core.Abstractions;

namespace SachkovTech.Issues.Application.Features.Lessons.Command.AddTagToLesson;

public record AddTagToLessonCommand(Guid LessonId, Guid TagId) : ICommand;