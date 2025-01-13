using CSharpFunctionalExtensions;
using Microsoft.Extensions.Logging;
using SachkovTech.Core.Abstractions;
using SachkovTech.Core.Database;
using SachkovTech.Issues.Application.Interfaces;
using SharedKernel;

namespace SachkovTech.Issues.Application.Features.Lessons.Command.AddTagToLesson;

public class AddTagToLessonHandler : ICommandHandler<AddTagToLessonCommand>
{
    private readonly ILessonsRepository _lessonsRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<AddTagToLessonHandler> _logger;

    public AddTagToLessonHandler(
        ILessonsRepository lessonsRepository,
        IUnitOfWork unitOfWork,
        ILogger<AddTagToLessonHandler> logger)
    {
        _lessonsRepository = lessonsRepository;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<UnitResult<ErrorList>> Handle(AddTagToLessonCommand command, CancellationToken cancellationToken = default)
    {
        var lesson = await _lessonsRepository.GetById(command.LessonId, cancellationToken);
        if (lesson.IsFailure)
            return Errors.General.NotFound().ToErrorList();

        var result = lesson.Value.AddTag(command.TagId);
        if (result.IsFailure)
            return result.Error.ToErrorList();

        await _unitOfWork.SaveChanges(cancellationToken);

        _logger.Log(LogLevel.Information, "Added new tag with {TagId} to {LessonId}", command.TagId, command.LessonId);

        return UnitResult.Success<ErrorList>();
    }
}