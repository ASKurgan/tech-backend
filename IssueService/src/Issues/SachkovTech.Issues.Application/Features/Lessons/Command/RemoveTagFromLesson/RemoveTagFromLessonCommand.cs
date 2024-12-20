

using SachkovTech.Core.Abstractions;

namespace SachkovTech.Issues.Application.Features.Lessons.Command.RemoveTagFromLesson;

public record RemoveTagFromLessonCommand(Guid LessonId, Guid TagId) : ICommand;
