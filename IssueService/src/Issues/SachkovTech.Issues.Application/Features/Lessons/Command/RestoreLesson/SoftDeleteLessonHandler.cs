using CSharpFunctionalExtensions;
using Microsoft.Extensions.Logging;
using SachkovTech.Core.Abstractions;
using SachkovTech.Core.Database;
using SachkovTech.Issues.Application.Interfaces;
using SharedKernel;

namespace SachkovTech.Issues.Application.Features.Lessons.Command.RestoreLesson;

public class RestoreLessonHandler(
    ILessonsRepository lessonsRepository,
    IUnitOfWork unitOfWork,
    ILogger<RestoreLessonHandler> logger) : ICommandHandler<RestoreLessonCommand>
{
    public async Task<UnitResult<ErrorList>> Handle(
        RestoreLessonCommand command, CancellationToken cancellationToken = default)
    {
        var lesson = await lessonsRepository.GetById(command.LessonId, cancellationToken);
        if (lesson.IsFailure)
            return Errors.General.NotFound(command.LessonId, "lesson").ToErrorList();

        lesson.Value.Restore();


        await unitOfWork.SaveChanges(cancellationToken);

        logger.Log(LogLevel.Information, "Lesson with id {LessonId} restored", command.LessonId);

        return UnitResult.Success<ErrorList>();
    }
}