using CSharpFunctionalExtensions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using SachkovTech.Core.Abstractions;
using SachkovTech.Issues.Application.Interfaces;
using SachkovTech.SharedKernel;
using SharedKernel;

namespace SachkovTech.Issues.Application.Features.Lessons.Command.RemoveTagFromLesson;

public class RemoveTagFromLessonHandler(
    ILessonsRepository lessonsRepository,
    [FromKeyedServices(SharedKernel.Modules.Issues)] IUnitOfWork unitOfWork,
    ILogger<RemoveTagFromLessonHandler> logger) : ICommandHandler<RemoveTagFromLessonCommand>
{
    public async Task<UnitResult<ErrorList>> Handle(
        RemoveTagFromLessonCommand command, CancellationToken cancellationToken = default)
    {
        var lesson = await lessonsRepository.GetById(command.LessonId, cancellationToken);
        if (lesson.IsFailure)
            return Errors.General.NotFound(command.LessonId, "lesson").ToErrorList();

        var result = lesson.Value.RemoveTag(command.TagId);
        if (result.IsFailure)
            return result.Error.ToErrorList();

        await unitOfWork.SaveChanges(cancellationToken);

        logger.Log(LogLevel.Information, "Remove tag with {TagId} from {LessonId}", command.TagId, command.LessonId);

        return UnitResult.Success<ErrorList>();
    }
}
