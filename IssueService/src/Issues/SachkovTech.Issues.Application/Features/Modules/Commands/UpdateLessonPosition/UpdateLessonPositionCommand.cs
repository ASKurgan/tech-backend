using SachkovTech.Core.Abstractions;

namespace SachkovTech.Issues.Application.Features.Modules.Commands.UpdateLessonPosition;

public record UpdateLessonPositionCommand(Guid LessonId, Guid ModuleId, int Position) : ICommand;