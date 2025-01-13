using SachkovTech.Core.Abstractions;

namespace SachkovTech.Issues.Application.Features.Lessons.Command.RestoreLesson;

public record RestoreLessonCommand(Guid LessonId) : ICommand;