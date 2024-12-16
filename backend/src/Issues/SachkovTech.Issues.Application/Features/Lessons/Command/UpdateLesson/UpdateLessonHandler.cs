using CSharpFunctionalExtensions;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using SachkovTech.Core.Abstractions;
using SachkovTech.Core.Extensions;
using SachkovTech.Issues.Application.Interfaces;
using SachkovTech.Issues.Domain.Issue.ValueObjects;
using SachkovTech.SharedKernel;
using SachkovTech.SharedKernel.ValueObjects;
using SharedKernel;

namespace SachkovTech.Issues.Application.Features.Lessons.Command.UpdateLesson;

public class UpdateLessonHandler(
    IValidator<UpdateLessonCommand> validator,
    ILessonsRepository lessonsRepository,
    [FromKeyedServices(SharedKernel.Modules.Issues)] IUnitOfWork unitOfWork,
    ILogger<UpdateLessonHandler> logger) : ICommandHandler<UpdateLessonCommand>
{
    public async Task<UnitResult<ErrorList>> Handle(
        UpdateLessonCommand command, CancellationToken cancellationToken = default)
    {
        var validationResult = await validator.ValidateAsync(command, cancellationToken);
        if (validationResult.IsValid == false)
            return validationResult.ToList();

        var lesson = await lessonsRepository.GetById(command.LessonId, cancellationToken);
        if (lesson.IsFailure)
            return Errors.General.NotFound(command.LessonId, "lesson").ToErrorList();

        var title = Title.Create(command.Title).Value;
        var description = Description.Create(command.Title).Value;
        var experience = Experience.Create(command.Experience).Value;
        var video = new Video(command.VideoId);
        lesson.Value.Update(title, description, experience, video, command.PreviewId, command.Tags.ToArray(),
            command.Issues.ToArray());

        await unitOfWork.SaveChanges(cancellationToken);

        logger.Log(LogLevel.Information, "Updated lesson with {LessonId}", lesson.Value.Id);

        return UnitResult.Success<ErrorList>();
    }
}
