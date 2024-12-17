using CSharpFunctionalExtensions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using SachkovTech.Core.Abstractions;
using SachkovTech.Issues.Application.Interfaces;
using SharedKernel;

namespace SachkovTech.Issues.Application.Features.Lessons.Command.AddTagToLesson;

public class AddTagToLessonHandler(
    ILessonsRepository lessonsRepository,
    [FromKeyedServices(SharedKernel.Modules.Issues)] IUnitOfWork unitOfWork,
    ILogger<AddTagToLessonHandler> logger) : ICommandHandler<AddTagToLessonCommand>
{
    public async Task<UnitResult<ErrorList>> Handle(AddTagToLessonCommand command, CancellationToken cancellationToken = default)
    {
        var lesson = await lessonsRepository.GetById(command.LessonId, cancellationToken);
        if (lesson.IsFailure)
            return Errors.General.NotFound().ToErrorList();

        var result = lesson.Value.AddTag(command.TagId);
        if (result.IsFailure)
            return result.Error.ToErrorList();

        await unitOfWork.SaveChanges(cancellationToken);
        
        logger.Log(LogLevel.Information, "Added new tag with {TagId} to {LessonId}", command.TagId, command.LessonId);
        
        return UnitResult.Success<ErrorList>();
    }
}