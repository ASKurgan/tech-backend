using SachkovTech.Core.Abstractions;

namespace SachkovTech.Issues.Application.Features.Lessons.Command.SoftDeleteLesson;

public record SoftDeleteLessonCommand(Guid LessonId) : ICommand;