using CSharpFunctionalExtensions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using SachkovTech.Core.Abstractions;
using SachkovTech.Issues.Application.Features.Lessons.Command.AddIssueToLesson;
using SachkovTech.Issues.Application.Interfaces;
using SharedKernel;

namespace SachkovTech.Issues.Application.Features.Lessons.Command.SoftDeleteLesson;

public class SoftDeleteLessonHandler(
    ILessonsRepository lessonsRepository,
    [FromKeyedServices(SharedKernel.Modules.Issues)] IUnitOfWork unitOfWork,
    ILogger<AddIssueToLessonHandler> logger) : ICommandHandler<SoftDeleteLessonCommand>
{
    public async Task<UnitResult<ErrorList>> Handle(
        SoftDeleteLessonCommand command, CancellationToken cancellationToken = default)
    {
        var lesson = await lessonsRepository.GetById(command.LessonId, cancellationToken);
        if (lesson.IsFailure)
            return Errors.General.NotFound(command.LessonId, "lesson").ToErrorList();
        
        lesson.Value.SoftDelete();
        
        await unitOfWork.SaveChanges(cancellationToken);

        logger.Log(LogLevel.Information, "Lesson with id {LessonId} hided", command.LessonId);
        
        return UnitResult.Success<ErrorList>();
    }
}