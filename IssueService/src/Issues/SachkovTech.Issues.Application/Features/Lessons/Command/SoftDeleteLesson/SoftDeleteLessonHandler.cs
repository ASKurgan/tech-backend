using CSharpFunctionalExtensions;
using Microsoft.Extensions.Logging;
using SachkovTech.Core.Abstractions;
using SachkovTech.Core.Database;
using SachkovTech.Issues.Application.Features.Lessons.Command.AddIssueToLesson;
using SachkovTech.Issues.Application.Interfaces;
using SharedKernel;

namespace SachkovTech.Issues.Application.Features.Lessons.Command.SoftDeleteLesson;

public class SoftDeleteLessonHandler : ICommandHandler<SoftDeleteLessonCommand>
{
    private readonly ILessonsRepository _lessonsRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<AddIssueToLessonHandler> _logger;

    public SoftDeleteLessonHandler(
        ILessonsRepository lessonsRepository,
        IUnitOfWork unitOfWork,
        ILogger<AddIssueToLessonHandler> logger)
    {
        _lessonsRepository = lessonsRepository;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<UnitResult<ErrorList>> Handle(
        SoftDeleteLessonCommand command, CancellationToken cancellationToken = default)
    {
        var lesson = await _lessonsRepository.GetById(command.LessonId, cancellationToken);
        if (lesson.IsFailure)
            return Errors.General.NotFound(command.LessonId, "lesson").ToErrorList();

        lesson.Value.SoftDelete();

        await _unitOfWork.SaveChanges(cancellationToken);

        _logger.Log(LogLevel.Information, "Lesson with id {LessonId} hided", command.LessonId);

        return UnitResult.Success<ErrorList>();
    }
}